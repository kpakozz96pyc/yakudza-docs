import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';
import Login from './pages/Login';
import InitAdmin from './pages/InitAdmin';
import Feed from './pages/Feed';
import Dish from './pages/Dish';

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route path="/" element={<Navigate to="/feed" replace />} />
          <Route path="/login" element={<Login />} />
          <Route path="/init-admin" element={<InitAdmin />} />
          <Route path="/feed" element={<Feed />} />
          <Route
            path="/dish/:id"
            element={
              <ProtectedRoute>
                <Dish />
              </ProtectedRoute>
            }
          />
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
