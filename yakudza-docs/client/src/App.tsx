import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import Login from './pages/Login';
import InitAdmin from './pages/InitAdmin';
import Feed from './pages/Feed';
import Dish from './pages/Dish';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Navigate to="/feed" replace />} />
        <Route path="/login" element={<Login />} />
        <Route path="/init-admin" element={<InitAdmin />} />
        <Route path="/feed" element={<Feed />} />
        <Route path="/dish/:id" element={<Dish />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
