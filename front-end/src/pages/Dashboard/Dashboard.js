import { useState, useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import Navbar from '../../components/Navbar/Navbar';
import { useAuth } from '../../context/AuthContext';
import { dashboardService } from '../../services/dashboardService';
import styles from './_dashboard.module.css';

const avatarColors = [
  styles.avPurple,
  styles.avBlue,
  styles.avGreen,
  styles.avOrange,
  styles.avPink,
];

const prioridadeMap = {
  1: { label: 'Alta',  className: 'priorityHigh' },
  2: { label: 'Média', className: 'priorityMid'  },
  3: { label: 'Baixa', className: 'priorityLow'  },
};

const statusDotMap = {
  1: 'statusTodo',
  2: 'statusDoing',
  3: 'statusDone',
};

function getIniciais(nome) {
  if (!nome) return '??';
  const p = nome.trim().split(' ');
  if (p.length === 1) return p[0][0].toUpperCase();
  return (p[0][0] + p[p.length - 1][0]).toUpperCase();
}

function formatarData(iso) {
  if (!iso) return '';
  return new Date(iso).toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit' });
}

function isPrazoVencido(iso) {
  if (!iso) return false;
  return new Date(iso) < new Date();
}

function dataAtual() {
  const days   = ['Domingo','Segunda-feira','Terça-feira','Quarta-feira','Quinta-feira','Sexta-feira','Sábado'];
  const months = ['Janeiro','Fevereiro','Março','Abril','Maio','Junho','Julho','Agosto','Setembro','Outubro','Novembro','Dezembro'];
  const now    = new Date();
  return `${days[now.getDay()]}, ${now.getDate()} de ${months[now.getMonth()]} de ${now.getFullYear()}`;
}

function Dashboard() {
  const [resumo,     setResumo]     = useState(null);
  const [recentes,   setRecentes]   = useState([]);
  const [progresso,  setProgresso]  = useState(null);
  const [equipe,     setEquipe]     = useState([]);
  const [carregando, setCarregando] = useState(true);

  const { logout } = useAuth();
  const navigate   = useNavigate();
  const canvasRef  = useRef(null);

  // ── Stars ────────────────────────────────────────────────────────────────
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

  // ── Dados da API ─────────────────────────────────────────────────────────
  useEffect(() => {
    async function carregar() {
      try {
        const [r, rec, prog, eq] = await Promise.all([
          dashboardService.resumoTarefas(),
          dashboardService.tarefasRecentes(),
          dashboardService.progressoGeral(),
          dashboardService.resumoEquipe(),
        ]);
        setResumo(r);
        setRecentes(Array.isArray(rec) ? rec : []);
        setProgresso(prog);
        setEquipe(Array.isArray(eq) ? eq : []);
      } catch (e) {
        console.error('Erro ao carregar dashboard:', e);
      } finally {
        setCarregando(false);
      }
    }
    carregar();
  }, []);

  function handleLogout() {
    logout();
    navigate('/login');
  }

  // API retorna porcentagem diretamente
  const pct = progresso?.porcentagem ?? 0;

  return (
    <>
      <canvas ref={canvasRef} className={styles.canvas} />
      <div className={`${styles.nebula} ${styles.nebula1}`} />
      <div className={`${styles.nebula} ${styles.nebula2}`} />

      <Navbar />

      <main className={styles.main}>

        {/* HEADER */}
        <div className={styles.pageHeader}>
          <div>
            <h1 className={styles.pageTitle}>Dashboard</h1>
            <p className={styles.pageSub}>Bem-vindo de volta! Aqui está o resumo do projeto.</p>
          </div>
          <div className={styles.headerDate}>
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
              <rect x="3" y="4" width="18" height="18" rx="2"/>
              <line x1="16" y1="2" x2="16" y2="6"/>
              <line x1="8" y1="2" x2="8" y2="6"/>
              <line x1="3" y1="10" x2="21" y2="10"/>
            </svg>
            <span>{dataAtual()}</span>
          </div>
        </div>

        {/* SUMMARY CARDS */}
        <div className={styles.summaryGrid}>

          <div className={styles.summaryCard}>
            <div className={`${styles.summaryIcon} ${styles.iconTotal}`}>
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <rect x="3" y="3" width="7" height="7" rx="1"/><rect x="14" y="3" width="7" height="7" rx="1"/>
                <rect x="3" y="14" width="7" height="7" rx="1"/><rect x="14" y="14" width="7" height="7" rx="1"/>
              </svg>
            </div>
            <div className={styles.summaryInfo}>
              <span className={styles.summaryLabel}>Total de Tarefas</span>
              <span className={styles.summaryValue}>{carregando ? '—' : (resumo?.totalTarefas ?? 0)}</span>
            </div>
          </div>

          <div className={styles.summaryCard}>
            <div className={`${styles.summaryIcon} ${styles.iconDone}`}>
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/>
                <polyline points="22 4 12 14.01 9 11.01"/>
              </svg>
            </div>
            <div className={styles.summaryInfo}>
              <span className={styles.summaryLabel}>Concluídas</span>
              <span className={styles.summaryValue}>{carregando ? '—' : (resumo?.tarefasConcluidas ?? 0)}</span>
            </div>
          </div>

          <div className={styles.summaryCard}>
            <div className={`${styles.summaryIcon} ${styles.iconDoing}`}>
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <circle cx="12" cy="12" r="10"/>
                <polyline points="12 6 12 12 16 14"/>
              </svg>
            </div>
            <div className={styles.summaryInfo}>
              <span className={styles.summaryLabel}>Em Andamento</span>
              <span className={styles.summaryValue}>{carregando ? '—' : (resumo?.tarefasEmAndamento ?? 0)}</span>
            </div>
          </div>

          <div className={styles.summaryCard}>
            <div className={`${styles.summaryIcon} ${styles.iconTodo}`}>
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <circle cx="12" cy="12" r="10"/>
                <line x1="12" y1="8" x2="12" y2="12"/>
                <line x1="12" y1="16" x2="12.01" y2="16"/>
              </svg>
            </div>
            <div className={styles.summaryInfo}>
              <span className={styles.summaryLabel}>A Fazer</span>
              <span className={styles.summaryValue}>{carregando ? '—' : (resumo?.tarefasAFazer ?? 0)}</span>
            </div>
          </div>

        </div>

        {/* DASH GRID */}
        <div className={styles.dashGrid}>

          {/* TAREFAS RECENTES */}
          <div className={styles.dashCard}>
            <div className={styles.dashCardHeader}>
              <span className={styles.dashCardTitle}>Tarefas Recentes</span>
              <a href="/tarefas" className={styles.dashLink}>Ver todas</a>
            </div>

            {carregando ? (
              <p className={styles.textMuted}>Carregando...</p>
            ) : recentes.length === 0 ? (
              <p className={styles.textMuted}>Nenhuma tarefa recente.</p>
            ) : (
              <div className={styles.recentList}>
                {recentes.map(t => {
                  const prioridade = prioridadeMap[t.prioridade] || { label: '?', className: 'priorityHigh' };
                  const dotClass   = statusDotMap[t.status] || 'statusTodo';
                  const vencido    = isPrazoVencido(t.prazo) && t.status !== 3;
                  const concluida  = t.status === 3;

                  return (
                    <div key={t.id} className={styles.recentItem}>
                      <div className={styles.recentLeft}>
                        <div className={`${styles.recentStatus} ${styles[dotClass]}`} />
                        <div className={styles.recentInfo}>
                          <span className={styles.recentTitle}>{t.titulo}</span>
                          <span className={`${styles.recentMeta} ${vencido ? styles.overdue : ''}`}>
                            {t.responsavel ? `${t.responsavel} · ` : 'Sem responsável · '}
                            {concluida
                              ? 'concluída'
                              : vencido
                                ? `venceu ${formatarData(t.prazo)}`
                                : `vence ${formatarData(t.prazo)}`}
                          </span>
                        </div>
                      </div>
                      <span className={`${styles.priorityBadge} ${styles[prioridade.className]}`}>
                        {prioridade.label}
                      </span>
                    </div>
                  );
                })}
              </div>
            )}
          </div>

          {/* COLUNA DIREITA */}
          <div className={styles.dashRight}>

            {/* PROGRESSO GERAL */}
            <div className={styles.dashCard}>
              <div className={styles.dashCardHeader}>
                <span className={styles.dashCardTitle}>Progresso Geral</span>
                <span className={styles.progressPct}>{carregando ? '—' : `${pct}%`}</span>
              </div>
              <div className={styles.progressBarWrap}>
                <div className={styles.progressBar} style={{ width: carregando ? '0%' : `${pct}%` }} />
              </div>
              {/* Legenda usa resumo pois progressoGeral só retorna porcentagem */}
              <div className={styles.progressLegend}>
                <div className={styles.legendItem}>
                  <span className={`${styles.legendDot} ${styles.dotDone}`} />
                  <span>Concluídas <strong>{carregando ? '—' : (resumo?.tarefasConcluidas ?? 0)}</strong></span>
                </div>
                <div className={styles.legendItem}>
                  <span className={`${styles.legendDot} ${styles.dotDoing}`} />
                  <span>Em Andamento <strong>{carregando ? '—' : (resumo?.tarefasEmAndamento ?? 0)}</strong></span>
                </div>
                <div className={styles.legendItem}>
                  <span className={`${styles.legendDot} ${styles.dotTodo}`} />
                  <span>A Fazer <strong>{carregando ? '—' : (resumo?.tarefasAFazer ?? 0)}</strong></span>
                </div>
              </div>
            </div>

            {/* EQUIPE */}
            <div className={styles.dashCard}>
              <div className={styles.dashCardHeader}>
                <span className={styles.dashCardTitle}>Equipe</span>
                <a href="/usuarios" className={styles.dashLink}>Gerenciar</a>
              </div>

              {carregando ? (
                <p className={styles.textMuted}>Carregando...</p>
              ) : equipe.length === 0 ? (
                <p className={styles.textMuted}>Nenhum membro na equipe.</p>
              ) : (
                <div className={styles.teamList}>
                  {equipe.map((m, idx) => (
                    <div key={m.usuarioId ?? idx} className={styles.teamItem}>
                      <div className={`${styles.avatarSm} ${avatarColors[idx % avatarColors.length]}`}>
                        {getIniciais(m.nome)}
                      </div>
                      <div className={styles.teamInfo}>
                        <span className={styles.teamName}>{m.nome}</span>
                        {/* API retorna funcao como string diretamente */}
                        <span className={styles.teamRole}>{m.funcao ?? 'Membro'}</span>
                      </div>
                      <div className={styles.teamTasks}>
                        <span className={styles.taskCount}>
                          {m.totalTarefas ?? 0} tarefa{(m.totalTarefas ?? 0) !== 1 ? 's' : ''}
                        </span>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>

          </div>
        </div>

      </main>
    </>
  );
}

export default Dashboard;