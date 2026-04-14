import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import RotaProtegida from './components/RotaProtegida';
import Login from './pages/Login/Login';
import Usuarios from './pages/Usuarios/Usuario';
import Tarefas from './pages/Tarefas/Tarefas';
import Atribuicoes from './pages/Atribuicoes/Atribuicoes';
import Dashboard from './pages/Dashboard/Dashboard';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Navigate to="/login" />} />
        <Route path="/login" element={<Login />} />
        <Route path="/dashboard" element={<RotaProtegida><Dashboard /></RotaProtegida>} />
        <Route path="/usuarios" element={<RotaProtegida><Usuarios /></RotaProtegida>} />
        <Route path="/tarefas" element={<RotaProtegida><Tarefas /></RotaProtegida>} />
        <Route path="/atribuicoes" element={<RotaProtegida><Atribuicoes /></RotaProtegida>} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;