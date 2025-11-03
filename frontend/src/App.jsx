import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import AuthComponent from './components/auth/AuthComponent';
import Home from './components/home/Home';
export default function App() {




  return (
    <Router>
      <Routes>
        <Route path="/" element={<AuthComponent />} />
        <Route path="/home" element={<Home />} />
      </Routes>
    </Router>
    
  );
}