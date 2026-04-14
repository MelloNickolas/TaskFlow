import { useState, useEffect, useRef } from 'react';
import Navbar from '../../components/Navbar/Navbar';
import { usuarioService } from '../../services/usuarioService';
import styles from './_usuario.module.css';

// Cores dos avatares baseadas no índice
const avatarColors = [
  { background: 'linear-gradient(135deg,#5e17eb,#8c52ff)' },
  { background: 'linear-gradient(135deg,#1765eb,#52aaff)' },
  { background: 'linear-gradient(135deg,#17832e,#52ff7a)', color: '#001a05' },
  { background: 'linear-gradient(135deg,#eb7017,#ffb852)', color: '#1a0500' },
  { background: 'linear-gradient(135deg,#eb1782,#ff52c8)' },
];

// Mapeia funcao (número) para badge
const funcaoBadge = {
  1: { label: 'Desenvolvedor', className: styles.badgeDev },
  2: { label: 'Gerente', className: styles.badgeManager },
  3: { label: 'Administrador', className: styles.badgeAdmin },
};

// Gera iniciais do nome
function getIniciais(nome) {
  if (!nome) return '??';
  const partes = nome.trim().split(' ');
  if (partes.length === 1) return partes[0][0].toUpperCase();
  return (partes[0][0] + partes[partes.length - 1][0]).toUpperCase();
}

function Usuarios() {
  const [usuariosAtivos, setUsuariosAtivos] = useState([]);
  const [usuariosInativos, setUsuariosInativos] = useState([]);
  const [busca, setBusca] = useState('');
  const [carregando, setCarregando] = useState(true);
  const [erro, setErro] = useState(null);

  // Modal
  const [modalAberto, setModalAberto] = useState(false);
  const [modoEdicao, setModoEdicao] = useState(false);
  const [usuarioSelecionado, setUsuarioSelecionado] = useState(null);

  // Form
  const [formNome, setFormNome] = useState('');
  const [formEmail, setFormEmail] = useState('');
  const [formSenha, setFormSenha] = useState('');
  const [formFuncao, setFormFuncao] = useState('');
  const [formErro, setFormErro] = useState(null);

  const [modalSenhaAberto, setModalSenhaAberto] = useState(false);
  const [senhaAntiga, setSenhaAntiga] = useState('');
  const [senhaNova, setSenhaNova] = useState('');
  const [senhaErro, setSenhaErro] = useState(null);

  // Stars
  const canvasRef = useRef(null);

  useEffect(() => {
    const canvas = canvasRef.current;
    const ctx = canvas.getContext('2d');
    let W, H, stars = [], animId;

    function resize() { W = canvas.width = window.innerWidth; H = canvas.height = window.innerHeight; }
    function mkStar() { return { x: Math.random() * W, y: Math.random() * H, r: Math.random() * 1.2 + 0.2, a: Math.random(), speed: Math.random() * 0.003 + 0.001, phase: Math.random() * Math.PI * 2 }; }
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

  // Busca usuários da API
  useEffect(() => {
    carregarUsuarios();
  }, []);

  async function carregarUsuarios() {
    try {
      setCarregando(true);
      const [ativos, inativos] = await Promise.all([
        usuarioService.listar(true),
        usuarioService.listar(false),
      ]);
      setUsuariosAtivos(ativos);
      setUsuariosInativos(inativos);
    } catch {
      setErro('Erro ao carregar usuários.');
    } finally {
      setCarregando(false);
    }
  }

  const ativosFiltrados = usuariosAtivos.filter(u =>
    u.nome.toLowerCase().includes(busca.toLowerCase()) ||
    u.email.toLowerCase().includes(busca.toLowerCase())
  );

  const inativosFiltrados = usuariosInativos.filter(u =>
    u.nome.toLowerCase().includes(busca.toLowerCase()) ||
    u.email.toLowerCase().includes(busca.toLowerCase())
  );

  // Abre modal para criar
  function abrirModalCriar() {
    setModoEdicao(false);
    setUsuarioSelecionado(null);
    setFormNome(''); setFormEmail(''); setFormSenha(''); setFormFuncao('');
    setFormErro(null);
    setModalAberto(true);
  }

  // Abre modal para editar
  function abrirModalEditar(usuario) {
    setModoEdicao(true);
    setUsuarioSelecionado(usuario);
    setFormNome(usuario.nome);
    setFormEmail(usuario.email);
    setFormSenha('');
    setFormFuncao(String(usuario.funcao));
    setFormErro(null);
    setModalAberto(true);
  }

  function fecharModal() {
    setModalAberto(false);
  }

  async function handleSalvar() {
    setFormErro(null);
    try {
      if (modoEdicao) {
        await usuarioService.atualizar({
          id: usuarioSelecionado.id,
          nome: formNome,
          email: formEmail,
          funcao: Number(formFuncao),
        });
      } else {
        await usuarioService.criar({
          nome: formNome,
          email: formEmail,
          senha: formSenha,
          funcao: Number(formFuncao),
        });
      }
      fecharModal();
      carregarUsuarios();
    } catch {
      setFormErro('Erro ao salvar usuário. Verifique os dados.');
    }
  }

  async function handleDeletar(id) {
    if (!window.confirm('Deseja excluir este usuário?')) return;
    try {
      await usuarioService.deletar(id);
      carregarUsuarios();
    } catch {
      alert('Erro ao excluir usuário.');
    }
  }

  async function handleRestaurar(id) {
    if (!window.confirm('Deseja restaurar este usuário?')) return;
    try {
      await usuarioService.restaurar(id);
      carregarUsuarios();
    } catch {
      alert('Erro ao restaurar usuário.');
    }
  }

  function abrirModalSenha(usuario) {
    setUsuarioSelecionado(usuario);
    setSenhaAntiga('');
    setSenhaNova('');
    setSenhaErro(null);
    setModalSenhaAberto(true);
  }

  function fecharModalSenha() {
    setModalSenhaAberto(false);
  }

  async function handleAlterarSenha() {
    setSenhaErro(null);
    try {
      await usuarioService.atualizarSenha({
        id: usuarioSelecionado.id,
        senhaAntiga,
        senhaNova,
      });
      fecharModalSenha();
      alert('Senha alterada com sucesso!');
    } catch {
      setSenhaErro('Erro ao alterar senha. Verifique os dados.');
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
            <h1 className={styles.pageTitle}>Usuários</h1>
            <p className={styles.pageSub}>Gerencie os membros da equipe</p>
          </div>
          <button className={styles.btnPrimary} onClick={abrirModalCriar}>
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round"><line x1="12" y1="5" x2="12" y2="19" /><line x1="5" y1="12" x2="19" y2="12" /></svg>
            Novo Usuário
          </button>
        </div>

        <div className={styles.searchWrap}>
          <span className={styles.searchIcon}>
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><circle cx="11" cy="11" r="8" /><line x1="21" y1="21" x2="16.65" y2="16.65" /></svg>
          </span>
          <input
            type="text"
            className={styles.searchInput}
            placeholder="Buscar usuário por nome ou e-mail..."
            value={busca}
            onChange={(e) => setBusca(e.target.value)}
          />
        </div>

        {carregando && <p className={styles.textMuted}>Carregando...</p>}
        {erro && <p className={styles.erro}>{erro}</p>}

        {!carregando && (
          <>
            {/* ATIVOS */}
            <div className={styles.tableCard}>
              <table className={styles.usersTable}>
                <thead>
                  <tr>
                    <th>Usuário</th>
                    <th>E-mail</th>
                    <th>Função</th>
                    <th>Status</th>
                    <th>Ações</th>
                  </tr>
                </thead>
                <tbody>
                  {ativosFiltrados.map((u, index) => {
                    const cor = avatarColors[index % avatarColors.length];
                    const funcao = funcaoBadge[u.funcao] || { label: 'Desconhecido', className: '' };
                    return (
                      <tr key={u.id}>
                        <td>
                          <div className={styles.userCell}>
                            <div className={styles.avatar} style={cor}>{getIniciais(u.nome)}</div>
                            <span>{u.nome}</span>
                          </div>
                        </td>
                        <td className={styles.textMuted} data-label="E-mail">{u.email}</td>
                        <td data-label="Função">
                          <span className={`${styles.badge} ${funcao.className}`}>{funcao.label}</span>
                        </td>
                        <td data-label="Status">
                          <span className={`${styles.statusDot} ${styles.statusAtivo}`}></span>
                          Ativo
                        </td>
                        <td data-label="Ações">
                          <div className={styles.actions}>
                            <button className={styles.btnIcon} title="Alterar senha" onClick={() => abrirModalSenha(u)}>
                              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><rect x="3" y="11" width="18" height="11" rx="2" /><path d="M7 11V7a5 5 0 0 1 10 0v4" /></svg>
                            </button>
                            <button className={styles.btnIcon} title="Editar" onClick={() => abrirModalEditar(u)}>
                              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7" /><path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z" /></svg>
                            </button>
                            <button className={`${styles.btnIcon} ${styles.btnDanger}`} title="Excluir" onClick={() => handleDeletar(u.id)}>
                              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><polyline points="3 6 5 6 21 6" /><path d="M19 6l-1 14a2 2 0 0 1-2 2H8a2 2 0 0 1-2-2L5 6" /><path d="M10 11v6" /><path d="M14 11v6" /><path d="M9 6V4a1 1 0 0 1 1-1h4a1 1 0 0 1 1 1v2" /></svg>
                            </button>
                          </div>
                        </td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            </div>

            {/* INATIVOS */}
            {inativosFiltrados.length > 0 && (
              <>
                <div className={styles.inativosHeader}>
                  <span>Usuários Inativos</span>
                </div>
                <div className={`${styles.tableCard} ${styles.tableInativos}`}>
                  <table className={styles.usersTable}>
                    <thead>
                      <tr>
                        <th>Usuário</th>
                        <th>E-mail</th>
                        <th>Função</th>
                        <th>Status</th>
                        <th>Ações</th>
                      </tr>
                    </thead>
                    <tbody>
                      {inativosFiltrados.map((u, index) => {
                        const cor = avatarColors[index % avatarColors.length];
                        const funcao = funcaoBadge[u.funcao] || { label: 'Desconhecido', className: '' };
                        return (
                          <tr key={u.id}>
                            <td>
                              <div className={styles.userCell}>
                                <div className={styles.avatar} style={cor}>{getIniciais(u.nome)}</div>
                                <span>{u.nome}</span>
                              </div>
                            </td>
                            <td className={styles.textMuted} data-label="E-mail">{u.email}</td>
                            <td data-label="Função">
                              <span className={`${styles.badge} ${funcao.className}`}>{funcao.label}</span>
                            </td>
                            <td data-label="Status">
                              <span className={`${styles.statusDot} ${styles.statusInativo}`}></span>
                              Inativo
                            </td>
                            <td data-label="Ações">
                              <div className={styles.actions}>
                                <button className={`${styles.btnIcon} ${styles.btnRestore}`} title="Restaurar" onClick={() => handleRestaurar(u.id)}>
                                  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="M3 12a9 9 0 1 0 9-9 9.75 9.75 0 0 0-6.74 2.74L3 8" /><path d="M3 3v5h5" /></svg>
                                </button>
                              </div>
                            </td>
                          </tr>
                        );
                      })}
                    </tbody>
                  </table>
                </div>
              </>
            )}
          </>
        )}
      </main>

      {/* MODAL */}
      <div
        className={`${styles.modalOverlay} ${modalAberto ? styles.modalOverlayOpen : ''}`}
        onClick={(e) => { if (e.target === e.currentTarget) fecharModal(); }}
      >
        <div className={`${styles.modalBox} ${modalAberto ? styles.modalBoxOpen : ''}`}>
          <div className={styles.modalHeader}>
            <h2 className={styles.modalTitle}>{modoEdicao ? 'Editar Usuário' : 'Novo Usuário'}</h2>
            <button className={styles.modalClose} onClick={fecharModal}>
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round"><line x1="18" y1="6" x2="6" y2="18" /><line x1="6" y1="6" x2="18" y2="18" /></svg>
            </button>
          </div>

          <div className={styles.modalBody}>
            {formErro && <p className={styles.erro}>{formErro}</p>}

            <div className={styles.formGroup}>
              <label>Nome completo</label>
              <div className={styles.inputWrap}>
                <span className={styles.inputIcon}>
                  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><circle cx="12" cy="8" r="4" /><path d="M20 21a8 8 0 0 0-16 0" /></svg>
                </span>
                <input type="text" placeholder="Ex: Ana Lima" value={formNome} onChange={(e) => setFormNome(e.target.value)} />
              </div>
            </div>

            <div className={styles.formGroup}>
              <label>E-mail</label>
              <div className={styles.inputWrap}>
                <span className={styles.inputIcon}>
                  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><rect x="2" y="4" width="20" height="16" rx="2" /><path d="m22 7-8.97 5.7a1.94 1.94 0 0 1-2.06 0L2 7" /></svg>
                </span>
                <input type="text" placeholder="email@taskflow.com" value={formEmail} onChange={(e) => setFormEmail(e.target.value)} />
              </div>
            </div>

            {!modoEdicao && (
              <div className={styles.formGroup}>
                <label>Senha</label>
                <div className={styles.inputWrap}>
                  <span className={styles.inputIcon}>
                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><rect x="3" y="11" width="18" height="11" rx="2" /><path d="M7 11V7a5 5 0 0 1 10 0v4" /></svg>
                  </span>
                  <input type="text" placeholder="••••••••" value={formSenha} onChange={(e) => setFormSenha(e.target.value)} />
                </div>
              </div>
            )}

            <div className={styles.formGroup}>
              <label>Função</label>
              <div className={styles.selectWrap}>
                <select value={formFuncao} onChange={(e) => setFormFuncao(e.target.value)}>
                  <option value="" disabled>Selecione uma função</option>
                  <option value="1">Desenvolvedor</option>
                  <option value="2">Gerente</option>
                  <option value="3">Administrador</option>
                </select>
                <span className={styles.selectArrow}>
                  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round"><polyline points="6 9 12 15 18 9" /></svg>
                </span>
              </div>
            </div>
          </div>

          <div className={styles.modalFooter}>
            <button className={styles.btnSecondary} onClick={fecharModal}>Cancelar</button>
            <button className={styles.btnPrimary} onClick={handleSalvar}>Salvar Usuário</button>
          </div>
        </div>



      </div>

      <div
        className={`${styles.modalOverlay} ${modalSenhaAberto ? styles.modalOverlayOpen : ''}`}
        onClick={(e) => { if (e.target === e.currentTarget) fecharModalSenha(); }}
      >
        <div className={`${styles.modalBox} ${modalSenhaAberto ? styles.modalBoxOpen : ''}`}>
          <div className={styles.modalHeader}>
            <h2 className={styles.modalTitle}>Alterar Senha</h2>
            <button className={styles.modalClose} onClick={fecharModalSenha}>
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round"><line x1="18" y1="6" x2="6" y2="18" /><line x1="6" y1="6" x2="18" y2="18" /></svg>
            </button>
          </div>
          <div className={styles.modalBody}>
            {senhaErro && <p className={styles.erro}>{senhaErro}</p>}
            <div className={styles.formGroup}>
              <label>Senha Antiga</label>
              <div className={styles.inputWrap}>
                <span className={styles.inputIcon}>
                  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><rect x="3" y="11" width="18" height="11" rx="2" /><path d="M7 11V7a5 5 0 0 1 10 0v4" /></svg>
                </span>
                <input type="password" placeholder="••••••••" value={senhaAntiga} onChange={(e) => setSenhaAntiga(e.target.value)} />
              </div>
            </div>
            <div className={styles.formGroup}>
              <label>Nova Senha</label>
              <div className={styles.inputWrap}>
                <span className={styles.inputIcon}>
                  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><rect x="3" y="11" width="18" height="11" rx="2" /><path d="M7 11V7a5 5 0 0 1 10 0v4" /></svg>
                </span>
                <input type="password" placeholder="••••••••" value={senhaNova} onChange={(e) => setSenhaNova(e.target.value)} />
              </div>
            </div>
          </div>
          <div className={styles.modalFooter}>
            <button className={styles.btnSecondary} onClick={fecharModalSenha}>Cancelar</button>
            <button className={styles.btnPrimary} onClick={handleAlterarSenha}>Salvar Senha</button>
          </div>
        </div>
      </div>
    </>
  );
}

export default Usuarios;