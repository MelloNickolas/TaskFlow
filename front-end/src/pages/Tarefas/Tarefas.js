import { useState, useEffect, useRef } from 'react';
import Navbar from '../../components/Navbar/Navbar';
import { tarefaService } from '../../services/tarefaService';
import { usuarioService } from '../../services/usuarioService';
import { atribuicaoService } from '../../services/atribuicaoService';
import styles from './_tarefa.module.css';

const prioridadeMap = {
  1: { label: 'Alta',  className: styles.priorityHigh },
  2: { label: 'Média', className: styles.priorityMid  },
  3: { label: 'Baixa', className: styles.priorityLow  },
};

const statusMap = {
  1: { label: 'A Fazer',      className: styles.statusTodo  },
  2: { label: 'Em Andamento', className: styles.statusDoing },
  3: { label: 'Concluído',    className: styles.statusDone  },
};

// Próximo status no fluxo: A Fazer → Em Andamento → Concluído
const proximoStatus = { 1: 2, 2: 3, 3: 1 };
const proximoStatusLabel = { 1: 'Iniciar', 2: 'Concluir', 3: 'Reabrir' };
const proximoStatusIcon = {
  1: (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
      <polygon points="5 3 19 12 5 21 5 3"/>
    </svg>
  ),
  2: (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
      <polyline points="20 6 9 17 4 12"/>
    </svg>
  ),
  3: (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
      <polyline points="1 4 1 10 7 10"/><path d="M3.51 15a9 9 0 1 0 .49-3.5"/>
    </svg>
  ),
};

const avatarColors = [
  { background: 'linear-gradient(135deg,#5e17eb,#8c52ff)' },
  { background: 'linear-gradient(135deg,#1765eb,#52aaff)' },
  { background: 'linear-gradient(135deg,#17832e,#52ff7a)', color: '#001a05' },
  { background: 'linear-gradient(135deg,#eb7017,#ffb852)', color: '#1a0500' },
  { background: 'linear-gradient(135deg,#eb1782,#ff52c8)' },
];

function getIniciais(nome) {
  if (!nome) return '??';
  const partes = nome.trim().split(' ');
  if (partes.length === 1) return partes[0][0].toUpperCase();
  return (partes[0][0] + partes[partes.length - 1][0]).toUpperCase();
}

function formatarPrazo(prazo) {
  if (!prazo) return '';
  return new Date(prazo).toLocaleDateString('pt-BR');
}

function isPrazoVencido(prazo) {
  if (!prazo) return false;
  return new Date(prazo) < new Date();
}

function Tarefas() {
  const [tarefas, setTarefas] = useState([]);
  const [usuarios, setUsuarios] = useState([]);
  const [atribuicoesPorTarefa, setAtribuicoesPorTarefa] = useState({});
  const [carregando, setCarregando] = useState(true);
  const [erro, setErro] = useState(null);
  const [atualizandoStatus, setAtualizandoStatus] = useState(null); // id da tarefa sendo atualizada

  // Filtros
  const [filtroStatus, setFiltroStatus] = useState('');
  const [filtroPrioridade, setFiltroPrioridade] = useState('');
  const [filtroUsuario, setFiltroUsuario] = useState('');
  const [busca, setBusca] = useState('');

  // Modal
  const [modalAberto, setModalAberto] = useState(false);
  const [modoEdicao, setModoEdicao] = useState(false);
  const [tarefaSelecionada, setTarefaSelecionada] = useState(null);

  // Form
  const [formTitulo, setFormTitulo] = useState('');
  const [formDescricao, setFormDescricao] = useState('');
  const [formPrazo, setFormPrazo] = useState('');
  const [formPrioridade, setFormPrioridade] = useState('');
  const [formStatus, setFormStatus] = useState('');
  const [formErro, setFormErro] = useState(null);

  const canvasRef = useRef(null);

  // Stars
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
        const alpha = 0.25 + 0.75 * (0.5 + 0.5 * Math.sin(s.phase + t * s.speed));
        ctx.beginPath(); ctx.arc(s.x, s.y, s.r, 0, Math.PI * 2);
        ctx.fillStyle = `rgba(200,170,255,${alpha * s.a})`; ctx.fill();
      });
      animId = requestAnimationFrame(drawStars);
    }
    resize(); initStars(); animId = requestAnimationFrame(drawStars);
    window.addEventListener('resize', () => { resize(); initStars(); });
    return () => { cancelAnimationFrame(animId); window.removeEventListener('resize', resize); };
  }, []);

  useEffect(() => {
    carregarDados();
  }, []);

  async function carregarDados() {
    try {
      setCarregando(true);
      const [tarefasData, usuariosData] = await Promise.all([
        tarefaService.listar(),
        usuarioService.listar(true),
      ]);
      setTarefas(tarefasData);
      setUsuarios(usuariosData);

      const atribMap = {};
      await Promise.all(
        tarefasData.map(async (t) => {
          try {
            const rels = await atribuicaoService.obterPorTarefa(t.id);
            atribMap[t.id] = (rels || []).map((r, idx) => {
              const usuario = usuariosData.find(u => u.id === r.usuarioId);
              return {
                usuarioId: r.usuarioId,
                nome: usuario ? usuario.nome : 'Usuário',
                cor: avatarColors[idx % avatarColors.length],
              };
            });
          } catch {
            atribMap[t.id] = [];
          }
        })
      );
      setAtribuicoesPorTarefa(atribMap);
    } catch {
      setErro('Erro ao carregar tarefas.');
    } finally {
      setCarregando(false);
    }
  }

  // Avança para o próximo status sem abrir modal
  async function handleAvancarStatus(tarefa) {
    const novoStatus = proximoStatus[tarefa.status];
    setAtualizandoStatus(tarefa.id);
    try {
      await tarefaService.atualizarStatus({ id: tarefa.id, status: novoStatus });
      // Atualiza localmente sem refetch completo
      setTarefas(prev =>
        prev.map(t => t.id === tarefa.id ? { ...t, status: novoStatus } : t)
      );
    } catch {
      alert('Erro ao atualizar status.');
    } finally {
      setAtualizandoStatus(null);
    }
  }

  const tarefasFiltradas = tarefas.filter(t => {
    const matchBusca      = t.titulo.toLowerCase().includes(busca.toLowerCase());
    const matchStatus     = filtroStatus === '' || t.status === Number(filtroStatus);
    const matchPrioridade = filtroPrioridade === '' || t.prioridade === Number(filtroPrioridade);
    const matchUsuario    = filtroUsuario === '' ||
      (atribuicoesPorTarefa[t.id] || []).some(a => String(a.usuarioId) === String(filtroUsuario));
    return matchBusca && matchStatus && matchPrioridade && matchUsuario;
  });

  function abrirModalCriar() {
    setModoEdicao(false);
    setTarefaSelecionada(null);
    setFormTitulo(''); setFormDescricao(''); setFormPrazo('');
    setFormPrioridade(''); setFormStatus('');
    setFormErro(null);
    setModalAberto(true);
  }

  function abrirModalEditar(tarefa) {
    setModoEdicao(true);
    setTarefaSelecionada(tarefa);
    setFormTitulo(tarefa.titulo);
    setFormDescricao(tarefa.descricao || '');
    setFormPrazo(tarefa.prazo ? tarefa.prazo.split('T')[0] : '');
    setFormPrioridade(String(tarefa.prioridade));
    setFormStatus(String(tarefa.status));
    setFormErro(null);
    setModalAberto(true);
  }

  function fecharModal() { setModalAberto(false); }

  async function handleSalvar() {
    setFormErro(null);
    try {
      if (modoEdicao) {
        await tarefaService.atualizar({
          id: tarefaSelecionada.id,
          titulo: formTitulo,
          descricao: formDescricao,
          prazo: formPrazo,
          prioridade: Number(formPrioridade),
          status: Number(formStatus),
        });
      } else {
        await tarefaService.criar({
          titulo: formTitulo,
          descricao: formDescricao,
          prazo: formPrazo,
          prioridade: Number(formPrioridade),
        });
      }
      fecharModal();
      carregarDados();
    } catch {
      setFormErro('Erro ao salvar tarefa. Verifique os dados.');
    }
  }

  async function handleDeletar(id) {
    if (!window.confirm('Deseja excluir esta tarefa?')) return;
    try {
      await tarefaService.deletar(id);
      carregarDados();
    } catch {
      alert('Erro ao excluir tarefa.');
    }
  }

  return (
    <>
      <canvas ref={canvasRef} className={styles.canvas} />
      <div className={`${styles.nebula} ${styles.nebula1}`} />
      <div className={`${styles.nebula} ${styles.nebula2}`} />

      <Navbar />

      <main className={styles.main}>

        <div className={styles.pageHeader}>
          <div>
            <h1 className={styles.pageTitle}>Tarefas</h1>
            <p className={styles.pageSub}>Acompanhe e gerencie todas as tarefas</p>
          </div>
          <button className={styles.btnPrimary} onClick={abrirModalCriar}>
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round"><line x1="12" y1="5" x2="12" y2="19"/><line x1="5" y1="12" x2="19" y2="12"/></svg>
            Nova Tarefa
          </button>
        </div>

        {/* FILTROS */}
        <div className={styles.filtersBar}>
          <div className={styles.filterGroup}>
            <label className={styles.filterLabel}>Status</label>
            <div className={styles.selectWrap}>
              <select className={styles.filterSelect} value={filtroStatus} onChange={e => setFiltroStatus(e.target.value)}>
                <option value="">Todos</option>
                <option value="1">A Fazer</option>
                <option value="2">Em Andamento</option>
                <option value="3">Concluído</option>
              </select>
              <span className={styles.selectArrow}><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round"><polyline points="6 9 12 15 18 9"/></svg></span>
            </div>
          </div>

          <div className={styles.filterGroup}>
            <label className={styles.filterLabel}>Prioridade</label>
            <div className={styles.selectWrap}>
              <select className={styles.filterSelect} value={filtroPrioridade} onChange={e => setFiltroPrioridade(e.target.value)}>
                <option value="">Todas</option>
                <option value="1">Alta</option>
                <option value="2">Média</option>
                <option value="3">Baixa</option>
              </select>
              <span className={styles.selectArrow}><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round"><polyline points="6 9 12 15 18 9"/></svg></span>
            </div>
          </div>

          <div className={styles.filterGroup}>
            <label className={styles.filterLabel}>Responsável</label>
            <div className={styles.selectWrap}>
              <select className={styles.filterSelect} value={filtroUsuario} onChange={e => setFiltroUsuario(e.target.value)}>
                <option value="">Todos</option>
                {usuarios.map(u => (
                  <option key={u.id} value={u.id}>{u.nome}</option>
                ))}
              </select>
              <span className={styles.selectArrow}><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round"><polyline points="6 9 12 15 18 9"/></svg></span>
            </div>
          </div>

          <div className={styles.filterSearch}>
            <span className={styles.filterSearchLabel}>Buscar</span>
            <span className={styles.searchIcon}><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/></svg></span>
            <input type="text" className={styles.searchInput} placeholder="Buscar tarefa..." value={busca} onChange={e => setBusca(e.target.value)} />
          </div>
        </div>

        {carregando && <p className={styles.erro}>Carregando...</p>}
        {erro && <p className={styles.erro}>{erro}</p>}

        {!carregando && (
          <>
            {tarefasFiltradas.length === 0 ? (
              <p className={styles.emptyState}>Nenhuma tarefa encontrada.</p>
            ) : (
              <div className={styles.tasksGrid}>
                {tarefasFiltradas.map((t) => {
                  const prioridade  = prioridadeMap[t.prioridade] || { label: '?', className: '' };
                  const status      = statusMap[t.status]         || { label: '?', className: '' };
                  const vencido     = isPrazoVencido(t.prazo) && t.status !== 3;
                  const atribs      = atribuicoesPorTarefa[t.id] || [];
                  const primario    = atribs[0] || null;
                  const carregandoEste = atualizandoStatus === t.id;

                  return (
                    <div key={t.id} className={styles.taskCard}>
                      <div className={styles.taskCardHeader}>
                        <span className={`${styles.priorityBadge} ${prioridade.className}`}>{prioridade.label}</span>
                        <span className={`${styles.statusBadge} ${status.className}`}>{status.label}</span>
                      </div>

                      <h3 className={styles.taskTitle}>{t.titulo}</h3>
                      <p className={styles.taskDesc}>{t.descricao}</p>

                      <div className={styles.taskMeta}>
                        <div className={styles.taskAssignee}>
                          {primario ? (
                            <>
                              <div className={styles.avatarSm} style={primario.cor}>
                                {getIniciais(primario.nome)}
                              </div>
                              <span>{primario.nome}</span>
                              {atribs.length > 1 && (
                                <span className={styles.maisAtribs}>+{atribs.length - 1}</span>
                              )}
                            </>
                          ) : (
                            <span className={styles.semResponsavel}>Sem responsável</span>
                          )}
                        </div>
                        <div className={`${styles.taskDue} ${vencido ? styles.taskDueOverdue : ''}`}>
                          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><rect x="3" y="4" width="18" height="18" rx="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/></svg>
                          {formatarPrazo(t.prazo)}
                        </div>
                      </div>

                      <div className={styles.taskActions}>
                        {/* BOTÃO AVANÇAR STATUS */}
                        <button
                          className={`${styles.btnStatus} ${styles[`btnStatus${t.status}`]}`}
                          title={proximoStatusLabel[t.status]}
                          onClick={() => handleAvancarStatus(t)}
                          disabled={carregandoEste}
                        >
                          {carregandoEste ? (
                            <svg className={styles.spinIcon} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><line x1="12" y1="2" x2="12" y2="6"/><line x1="12" y1="18" x2="12" y2="22"/><line x1="4.93" y1="4.93" x2="7.76" y2="7.76"/><line x1="16.24" y1="16.24" x2="19.07" y2="19.07"/><line x1="2" y1="12" x2="6" y2="12"/><line x1="18" y1="12" x2="22" y2="12"/><line x1="4.93" y1="19.07" x2="7.76" y2="16.24"/><line x1="16.24" y1="7.76" x2="19.07" y2="4.93"/></svg>
                          ) : proximoStatusIcon[t.status]}
                          {proximoStatusLabel[t.status]}
                        </button>

                        <div className={styles.taskActionsRight}>
                          <button className={styles.btnIcon} title="Editar" onClick={() => abrirModalEditar(t)}>
                            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"/><path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"/></svg>
                          </button>
                          <button className={`${styles.btnIcon} ${styles.btnDanger}`} title="Excluir" onClick={() => handleDeletar(t.id)}>
                            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><polyline points="3 6 5 6 21 6"/><path d="M19 6l-1 14a2 2 0 0 1-2 2H8a2 2 0 0 1-2-2L5 6"/><path d="M10 11v6"/><path d="M14 11v6"/><path d="M9 6V4a1 1 0 0 1 1-1h4a1 1 0 0 1 1 1v2"/></svg>
                          </button>
                        </div>
                      </div>
                    </div>
                  );
                })}
              </div>
            )}
          </>
        )}
      </main>

      {/* MODAL CRIAR / EDITAR */}
      <div
        className={`${styles.modalOverlay} ${modalAberto ? styles.modalOverlayOpen : ''}`}
        onClick={(e) => { if (e.target === e.currentTarget) fecharModal(); }}
      >
        <div className={`${styles.modalBox} ${modalAberto ? styles.modalBoxOpen : ''}`}>
          <div className={styles.modalHeader}>
            <h2 className={styles.modalTitle}>{modoEdicao ? 'Editar Tarefa' : 'Nova Tarefa'}</h2>
            <button className={styles.modalClose} onClick={fecharModal}>
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round"><line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/></svg>
            </button>
          </div>

          <div className={styles.modalBody}>
            {formErro && <p className={styles.erro}>{formErro}</p>}

            <div className={styles.formGroup}>
              <label>Título</label>
              <div className={styles.inputWrap}>
                <span className={styles.inputIcon}>
                  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="M12 20h9"/><path d="M16.5 3.5a2.121 2.121 0 0 1 3 3L7 19l-4 1 1-4L16.5 3.5z"/></svg>
                </span>
                <input type="text" placeholder="Ex: Redesign da tela de login" value={formTitulo} onChange={e => setFormTitulo(e.target.value)} />
              </div>
            </div>

            <div className={styles.formGroup}>
              <label>Descrição</label>
              <textarea className={styles.textarea} placeholder="Descreva o que precisa ser feito..." value={formDescricao} onChange={e => setFormDescricao(e.target.value)} />
            </div>

            <div className={styles.formRow}>
              <div className={styles.formGroup}>
                <label>Prazo</label>
                <div className={styles.inputWrap}>
                  <span className={styles.inputIcon}>
                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><rect x="3" y="4" width="18" height="18" rx="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/></svg>
                  </span>
                  <input type="date" value={formPrazo} onChange={e => setFormPrazo(e.target.value)} />
                </div>
              </div>

              <div className={styles.formGroup}>
                <label>Prioridade</label>
                <div className={styles.selectWrap}>
                  <select value={formPrioridade} onChange={e => setFormPrioridade(e.target.value)}>
                    <option value="" disabled>Selecione</option>
                    <option value="1">Alta</option>
                    <option value="2">Média</option>
                    <option value="3">Baixa</option>
                  </select>
                  <span className={styles.selectArrow}><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round"><polyline points="6 9 12 15 18 9"/></svg></span>
                </div>
              </div>
            </div>

            {modoEdicao && (
              <div className={styles.formGroup}>
                <label>Status</label>
                <div className={styles.selectWrap}>
                  <select value={formStatus} onChange={e => setFormStatus(e.target.value)}>
                    <option value="" disabled>Selecione</option>
                    <option value="1">A Fazer</option>
                    <option value="2">Em Andamento</option>
                    <option value="3">Concluído</option>
                  </select>
                  <span className={styles.selectArrow}><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round"><polyline points="6 9 12 15 18 9"/></svg></span>
                </div>
              </div>
            )}
          </div>

          <div className={styles.modalFooter}>
            <button className={styles.btnSecondary} onClick={fecharModal}>Cancelar</button>
            <button className={styles.btnPrimary} onClick={handleSalvar}>Salvar Tarefa</button>
          </div>
        </div>
      </div>
    </>
  );
}

export default Tarefas;