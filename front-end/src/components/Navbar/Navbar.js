import { useAuth } from '../../context/AuthContext';
import { NavLink } from 'react-router-dom';
import styles from './_navbar.module.css';
import { useNavigate } from 'react-router-dom';


function Navbar() {
  const { logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };
  return (
    <>
      {/* NAVBAR TOP */}
      <nav className={styles.navbar}>
        <div className={styles.navLogo}>
          <span>TaskFlow</span>
        </div>

        <ul className={styles.navLinks}>
          <li><NavLink to="/dashboard" className={({ isActive }) => isActive ? `${styles.navLink} ${styles.active}` : styles.navLink}>Dashboard</NavLink></li>
          <li><NavLink to="/usuarios" className={({ isActive }) => isActive ? `${styles.navLink} ${styles.active}` : styles.navLink}>Usuários</NavLink></li>
          <li><NavLink to="/tarefas" className={({ isActive }) => isActive ? `${styles.navLink} ${styles.active}` : styles.navLink}>Tarefas</NavLink></li>
          <li><NavLink to="/atribuicoes" className={({ isActive }) => isActive ? `${styles.navLink} ${styles.active}` : styles.navLink}>Atribuições</NavLink></li>
        </ul>

        <div className={styles.navRight}>
          <button className={styles.btnLogout} onClick={handleLogout}>
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" /><polyline points="16 17 21 12 16 7" /><line x1="21" y1="12" x2="9" y2="12" /></svg>
            Sair
          </button>
        </div>
      </nav>

      {/* NAVBAR BOTTOM (mobile) */}
      <nav className={styles.navbarMobile}>
        <NavLink to="/dashboard" className={({ isActive }) => isActive ? `${styles.mobLink} ${styles.active}` : styles.mobLink}>
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><rect x="3" y="3" width="7" height="7" rx="1" /><rect x="14" y="3" width="7" height="7" rx="1" /><rect x="3" y="14" width="7" height="7" rx="1" /><rect x="14" y="14" width="7" height="7" rx="1" /></svg>
          <span>Dashboard</span>
        </NavLink>
        <NavLink to="/usuarios" className={({ isActive }) => isActive ? `${styles.mobLink} ${styles.active}` : styles.mobLink}>
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><circle cx="9" cy="7" r="4" /><path d="M3 21v-2a4 4 0 0 1 4-4h4a4 4 0 0 1 4 4v2" /><path d="M16 3.13a4 4 0 0 1 0 7.75" /><path d="M21 21v-2a4 4 0 0 0-3-3.85" /></svg>
          <span>Usuários</span>
        </NavLink>
        <NavLink to="/tarefas" className={({ isActive }) => isActive ? `${styles.mobLink} ${styles.active}` : styles.mobLink}>
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="M9 11l3 3L22 4" /><path d="M21 12v7a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11" /></svg>
          <span>Tarefas</span>
        </NavLink>
        <NavLink to="/atribuicoes" className={({ isActive }) => isActive ? `${styles.mobLink} ${styles.active}` : styles.mobLink}>
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" /><circle cx="9" cy="7" r="4" /><line x1="23" y1="11" x2="17" y2="11" /><line x1="20" y1="8" x2="20" y2="14" /></svg>
          <span>Atribuições</span>
        </NavLink>
      </nav>
    </>
  );
}

export default Navbar;