import { useState, useEffect, useRef } from 'react';
import Navbar from '../../components/Navbar/Navbar';
import { atribuicaoService } from '../../services/atribuicaoService';
import { tarefaService } from '../../services/tarefaService';
import { usuarioService } from '../../services/usuarioService';
import styles from './_atribuicoes.module.css';

// Mapeia prioridade
const prioridadeMap = {
  1: { label: 'Alta',  className: 'priorityHigh' },
  2: { label: 'Média', className: 'priorityMid'  },
  3: { label: 'Baixa', className: 'priorityLow'  },
};

// Mapeia status
const statusMap = {
  1: { label: 'A Fazer',      className: 'statusTodo'  },
  2: { label: 'Em Andamento', className: 'statusDoing' },
  3: { label: 'Concluído',    className: 'statusDone'  },
};

// Cores dos avatares
const avatarColors = [
  styles.avPurple,
  styles.avBlue,
  styles.avGreen,
  styles.avOrange,
  styles.avPink,
];

function getIniciais(nome) {
  if (!nome) return '??';
  const partes = nome.trim().split(' ');
  if (partes.length === 1) return partes[0][0].toUpperCase();
  return (partes[0][0] + partes[partes.length - 1][0]).toUpperCase();
}

function Atribuicoes() {
  const [tarefas, setTarefas]   = useState([]);
  const [usuarios, setUsuarios] = useState([]);
  // atribuicoesPorTarefa: { [tarefaId]: [{ id, usuarioId, usuarioNome, corClass }] }
  const [atribuicoesPorTarefa, setAtribuicoesPorTarefa] = useState({});

  const [carregando, setCarregando] = useState(true);
  const [erro, setErro]             = useState(null);

  // Form
  const [tarefaId,  setTarefaId]  = useState('');
  const [usuarioId, setUsuarioId] = useState('');
  const [tarefaPreview, setTarefaPreview] = useState(null);
  const [filtroUsuario, setFiltroUsuario] = useState('');

  // Toast
  const [toast, setToast]       = useState({ msg: '', tipo: '', show: false });

  const canvasRef = useRef(null);

  // ── Stars ──────────────────────────────────────────────────────────────────
  useEffect(() => {
    const canvas = canvasRef.current;
    const ctx = canvas.getContext('2d');
    let W, H, stars = [], animId;
    function resize() { W = canvas.width = window.innerWidth; H = canvas.height = window.innerHeight; }
    function mkStar() { return { x: Math.random()*W, y: Math.random()*H, r: Math.random()*1.2+0.2, a: Math.random(), speed: Math.random()*0.003+0.001, phase: Math.random()*Math.PI*2 }; }
    function initStars(n = 180) { stars = Array.from({ length: n }, mkStar); }
    function drawStars(t) {
      ctx.clearRect(0, 0, W, H);
      stars.forEach(s => {
        const alpha = 0.25 + 0.75*(0.5 + 0.5*Math.sin(s.phase + t*s.speed));
        ctx.beginPath(); ctx.arc(s.x, s.y, s.r, 0, Math.PI*2);
        ctx.fillStyle = `rgba(200,170,255,${alpha*s.a})`; ctx.fill();
      });
      animId = requestAnimationFrame(drawStars);
    }
    resize(); initStars(); animId = requestAnimationFrame(drawStars);
    window.addEventListener('resize', () => { resize(); initStars(); });
    return () => { cancelAnimationFrame(animId); window.removeEventListener('resize', resize); };
  }, []);

  // ── Carrega dados iniciais ─────────────────────────────────────────────────
  useEffect(() => { carregarDados(); }, []);

  async function carregarDados() {
    try {
      setCarregando(true);
      const [tarefasData, usuariosData] = await Promise.all([
        tarefaService.listar(),
        usuarioService.listar(true),
      ]);
      setTarefas(tarefasData);
      setUsuarios(usuariosData);

      // Para cada tarefa, busca as atribuições
      const atribMap = {};
      await Promise.all(
        tarefasData.map(async (t) => {
          try {
            const rels = await atribuicaoService.obterPorTarefa(t.id);
            atribMap[t.id] = (rels || []).map((r, idx) => {
              const usuario = usuariosData.find(u => u.id === r.usuarioId);
              return {
                id:          r.id,
                usuarioId:   r.usuarioId,
                usuarioNome: usuario ? usuario.nome : 'Usuário',
                corClass:    avatarColors[idx % avatarColors.length],
              };
            });
          } catch {
            atribMap[t.id] = [];
          }
        })
      );
      setAtribuicoesPorTarefa(atribMap);
    } catch {
      setErro('Erro ao carregar dados.');
    } finally {
      setCarregando(false);
    }
  }

  // ── Preview da tarefa selecionada ──────────────────────────────────────────
  function onTarefaChange(e) {
    const id = e.target.value;
    setTarefaId(id);
    const t = tarefas.find(t => String(t.id) === String(id));
    setTarefaPreview(t || null);
  }

  // ── Atribuir responsável ───────────────────────────────────────────────────
  async function handleAtribuir() {
    if (!tarefaId || !usuarioId) {
      showToast('Selecione uma tarefa e um responsável.', 'warn');
      return;
    }

    const atribs = atribuicoesPorTarefa[tarefaId] || [];
    const jaAtribuido = atribs.some(a => String(a.usuarioId) === String(usuarioId));
    if (jaAtribuido) {
      const u = usuarios.find(u => String(u.id) === String(usuarioId));
      showToast(`${u?.nome || 'Usuário'} já está atribuído a esta tarefa.`, 'warn');
      return;
    }

    try {
      const rel = await atribuicaoService.criarRelacionamento({
        usuarioId: Number(usuarioId),
        tarefaId:  Number(tarefaId),
      });

      const usuario  = usuarios.find(u => String(u.id) === String(usuarioId));
      const novaAtrib = {
        id:          rel.id,
        usuarioId:   Number(usuarioId),
        usuarioNome: usuario?.nome || 'Usuário',
        corClass:    avatarColors[atribs.length % avatarColors.length],
      };

      setAtribuicoesPorTarefa(prev => ({
        ...prev,
        [tarefaId]: [...(prev[tarefaId] || []), novaAtrib],
      }));

      showToast(`${usuario?.nome || 'Usuário'} atribuído com sucesso!`, 'ok');
      setTarefaId('');
      setUsuarioId('');
      setTarefaPreview(null);
    } catch {
      showToast('Erro ao atribuir responsável.', 'warn');
    }
  }

  // ── Remover chip (atribuição individual) ──────────────────────────────────
  async function handleRemoverChip(tarefaId, atribId) {
    try {
      await atribuicaoService.deletarRelacionamento(atribId);
      setAtribuicoesPorTarefa(prev => ({
        ...prev,
        [tarefaId]: prev[tarefaId].filter(a => a.id !== atribId),
      }));
      showToast('Responsável removido.', 'warn');
    } catch {
      showToast('Erro ao remover responsável.', 'warn');
    }
  }

  // ── Remover todas as atribuições de uma tarefa ────────────────────────────
  async function handleRemoverTarefa(tarefaId) {
    const atribs = atribuicoesPorTarefa[tarefaId] || [];
    try {
      await Promise.all(atribs.map(a => atribuicaoService.deletarRelacionamento(a.id)));
      setAtribuicoesPorTarefa(prev => ({ ...prev, [tarefaId]: [] }));
      showToast('Todas as atribuições da tarefa foram removidas.', 'warn');
    } catch {
      showToast('Erro ao remover atribuições.', 'warn');
    }
  }

  // ── Toast ──────────────────────────────────────────────────────────────────
  function showToast(msg, tipo) {
    setToast({ msg, tipo, show: true });
    setTimeout(() => setToast(t => ({ ...t, show: false })), 3000);
  }

  // Filtra tarefas pelo usuário selecionado
  const tarefasFiltradas = filtroUsuario
    ? tarefas.filter(t =>
        (atribuicoesPorTarefa[t.id] || []).some(a => String(a.usuarioId) === String(filtroUsuario))
      )
    : tarefas;

  return (
    <>
      <canvas ref={canvasRef} className={styles.canvas} />
      <div className={`${styles.nebula} ${styles.nebula1}`} />
      <div className={`${styles.nebula} ${styles.nebula2}`} />

      <Navbar />

      <main className={styles.main}>

        <div className={styles.pageHeader}>
          <div>
            <h1 className={styles.pageTitle}>Atribuições</h1>
            <p className={styles.pageSub}>Vincule responsáveis às tarefas da equipe</p>
          </div>
        </div>

        <div className={styles.assignLayout}>

          {/* ── PAINEL ESQUERDO: Formulário ── */}
          <div className={styles.assignPanel}>
            <div className={styles.panelTitle}>
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><line x1="23" y1="11" x2="17" y2="11"/><line x1="20" y1="8" x2="20" y2="14"/></svg>
              Atribuir Responsável
            </div>

            <div className={styles.formGroup}>
              <label>Tarefa</label>
              <div className={styles.selectWrap}>
                <select value={tarefaId} onChange={onTarefaChange}>
                  <option value="" disabled>Selecione uma tarefa</option>
                  {tarefas.map(t => (
                    <option key={t.id} value={t.id}>{t.titulo}</option>
                  ))}
                </select>
                <span className={styles.selectArrow}>
                  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round"><polyline points="6 9 12 15 18 9"/></svg>
                </span>
              </div>
            </div>

            <div className={styles.formGroup}>
              <label>Responsável</label>
              <div className={styles.selectWrap}>
                <select value={usuarioId} onChange={e => setUsuarioId(e.target.value)}>
                  <option value="" disabled>Selecione um usuário</option>
                  {usuarios.map(u => (
                    <option key={u.id} value={u.id}>{u.nome}</option>
                  ))}
                </select>
                <span className={styles.selectArrow}>
                  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round"><polyline points="6 9 12 15 18 9"/></svg>
                </span>
              </div>
            </div>

            <button className={`${styles.btnPrimary} ${styles.btnFull}`} onClick={handleAtribuir}>
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round"><line x1="12" y1="5" x2="12" y2="19"/><line x1="5" y1="12" x2="19" y2="12"/></svg>
              Atribuir
            </button>

            {/* Preview da tarefa selecionada */}
            {tarefaPreview && (
              <div className={styles.taskPreview}>
                <div className={styles.previewLabel}>Tarefa selecionada</div>
                <div className={styles.previewTitle}>{tarefaPreview.titulo}</div>
                <div className={styles.previewMeta}>
                  Prioridade: {prioridadeMap[tarefaPreview.prioridade]?.label || '?'}
                  {'  •  '}
                  Status: {statusMap[tarefaPreview.status]?.label || '?'}
                </div>
              </div>
            )}
          </div>

          {/* ── PAINEL DIREITO: Lista de tarefas ── */}
          <div className={styles.tasksListPanel}>
            <div className={styles.panelTitle}>
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="M9 11l3 3L22 4"/><path d="M21 12v7a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11"/></svg>
              Tarefas e Responsáveis
            </div>

            {/* Filtro por usuário */}
            <div className={styles.filterBar}>
              <div className={styles.selectWrap}>
                <select value={filtroUsuario} onChange={e => setFiltroUsuario(e.target.value)}>
                  <option value="">Todos os responsáveis</option>
                  {usuarios.map(u => (
                    <option key={u.id} value={u.id}>{u.nome}</option>
                  ))}
                </select>
                <span className={styles.selectArrow}>
                  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round"><polyline points="6 9 12 15 18 9"/></svg>
                </span>
              </div>
            </div>

            {carregando && <p className={styles.textMuted}>Carregando...</p>}
            {erro && <p className={styles.erroMsg}>{erro}</p>}

            {!carregando && (
              <div className={styles.tasksAssignList}>
                {tarefasFiltradas.length === 0 && (
                  <p className={styles.textMuted}>Nenhuma tarefa encontrada.</p>
                )}
                {tarefasFiltradas.map(t => {
                  const prioridade = prioridadeMap[t.prioridade] || { label: '?', className: 'priorityHigh' };
                  const status     = statusMap[t.status]         || { label: '?', className: 'statusTodo'  };
                  const atribs     = atribuicoesPorTarefa[t.id]  || [];

                  return (
                    <div key={t.id} className={styles.taskAssignRow}>
                      <div className={styles.tarLeft}>
                        <div className={styles.tarInfo}>
                          <div className={styles.tarTitle}>{t.titulo}</div>
                          <div className={styles.tarBadges}>
                            <span className={`${styles.priorityBadge} ${styles[prioridade.className]}`}>
                              {prioridade.label}
                            </span>
                            <span className={`${styles.statusBadge} ${styles[status.className]}`}>
                              {status.label}
                            </span>
                          </div>
                        </div>
                      </div>

                      <div className={styles.tarRight}>
                        <div className={styles.assignees}>
                          {atribs.length === 0 ? (
                            <span className={styles.noAssignee}>Sem responsável</span>
                          ) : (
                            atribs.map(a => (
                              <div key={a.id} className={styles.avatarChip}>
                                <div className={`${styles.avatarSm} ${a.corClass}`}>
                                  {getIniciais(a.usuarioNome)}
                                </div>
                                <span>{a.usuarioNome}</span>
                                <button
                                  className={styles.chipRemove}
                                  title="Remover"
                                  onClick={() => handleRemoverChip(t.id, a.id)}
                                >
                                  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round"><line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/></svg>
                                </button>
                              </div>
                            ))
                          )}
                        </div>
                      </div>
                    </div>
                  );
                })}
              </div>
            )}
          </div>
        </div>
      </main>

      {/* Toast */}
      <div className={`${styles.toast} ${toast.show ? styles.toastShow : ''} ${toast.tipo === 'ok' ? styles.toastOk : styles.toastWarn}`}>
        {toast.msg}
      </div>
    </>
  );
}

export default Atribuicoes;