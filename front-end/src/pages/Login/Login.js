import { useState, useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import styles from './_login.module.css';

function Login() {
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [erro, setErro] = useState(null);

  const { login } = useAuth();
  const navigate = useNavigate();
  const canvasRef = useRef(null);

  // Star field
  useEffect(() => {
    const canvas = canvasRef.current;
    const ctx = canvas.getContext('2d');
    let W, H, stars = [];
    let animId;

    function resize() {
      W = canvas.width = window.innerWidth;
      H = canvas.height = window.innerHeight;
    }

    function mkStar() {
      return {
        x: Math.random() * W,
        y: Math.random() * H,
        r: Math.random() * 1.4 + 0.2,
        a: Math.random(),
        speed: Math.random() * 0.004 + 0.001,
        phase: Math.random() * Math.PI * 2,
      };
    }

    function initStars(n = 220) {
      stars = Array.from({ length: n }, mkStar);
    }

    function drawStars(t) {
      ctx.clearRect(0, 0, W, H);
      stars.forEach((s) => {
        const alpha = 0.3 + 0.7 * (0.5 + 0.5 * Math.sin(s.phase + t * s.speed));
        ctx.beginPath();
        ctx.arc(s.x, s.y, s.r, 0, Math.PI * 2);
        ctx.fillStyle = `rgba(200,170,255,${alpha * s.a})`;
        ctx.fill();
      });
      animId = requestAnimationFrame(drawStars);
    }

    resize();
    initStars();
    animId = requestAnimationFrame(drawStars);
    window.addEventListener('resize', () => { resize(); initStars(); });

    // Limpa ao desmontar o componente
    return () => {
      cancelAnimationFrame(animId);
      window.removeEventListener('resize', resize);
    };
  }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setErro(null);
    try {
      await login(email, senha);
      navigate('/dashboard');
    } catch {
      setErro('Email ou senha inválidos.');
    }
  };

  return (
    <>
      <canvas ref={canvasRef} className={styles.canvas} />
      <div className={`${styles.nebula} ${styles.nebula1}`} />
      <div className={`${styles.nebula} ${styles.nebula2}`} />
      <div className={`${styles.nebula} ${styles.nebula3}`} />

      <div className={styles.page}>

        {/* LEFT PANEL */}
        <div className={styles.leftPanel}>
          <div className={`${styles.orbitRing} ${styles.ring1}`} />
          <div className={`${styles.orbitRing} ${styles.ring2}`} />

          <p className={styles.tagline}>Gerencie além do horizonte</p>

          <h1 className={styles.heroText}>
            Organize o futuro,<br />uma tarefa por vez.
          </h1>

          <p className={styles.heroSub}>
            Acompanhe projetos, colabore com sua equipe e entregue resultados — com velocidade espacial.
          </p>
        </div>

        {/* DIVIDER */}
        <div className={styles.divider} />

        {/* RIGHT PANEL */}
        <div className={styles.rightPanel}>
          <div className={styles.card}>
            <h2 className={styles.cardTitle}>Bem-vindo de volta</h2>
            <p className={styles.cardSub}>Entre na sua conta para continuar</p>

            {erro && <p className={styles.erro}>{erro}</p>}

            <form onSubmit={handleSubmit}>
              {/* EMAIL */}
              <div className={styles.formGroup}>
                <label htmlFor="email">E-mail</label>
                <div className={styles.inputWrap}>
                  <span className={styles.inputIcon}>
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                      <rect x="2" y="4" width="20" height="16" rx="2"/><path d="m22 7-8.97 5.7a1.94 1.94 0 0 1-2.06 0L2 7"/>
                    </svg>
                  </span>
                  <input
                    type="email"
                    id="email"
                    placeholder="seu@email.com"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    required
                  />
                </div>
              </div>

              {/* SENHA */}
              <div className={styles.formGroup}>
                <label htmlFor="senha">Senha</label>
                <div className={styles.inputWrap}>
                  <span className={styles.inputIcon}>
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                      <rect x="3" y="11" width="18" height="11" rx="2"/><path d="M7 11V7a5 5 0 0 1 10 0v4"/>
                    </svg>
                  </span>
                  <input
                    type="password"
                    id="senha"
                    placeholder="••••••••"
                    value={senha}
                    onChange={(e) => setSenha(e.target.value)}
                    required
                  />
                </div>
              </div>

              <div className={styles.forgotRow}>
                <a href="#">Esqueceu a senha?</a>
              </div>

              <button type="submit" className={styles.btnPrimary}>
                Entrar
              </button>
            </form>
          </div>
        </div>

      </div>
    </>
  );
}

export default Login;