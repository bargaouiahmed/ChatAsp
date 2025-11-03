import UserList from "./UserList";
import ConversationsSidebar from "./ConversationsSidebar";
import { useNavigate } from "react-router-dom";
import './Home.css';
import signalRService from "../../services/SignalRService";

export default function Home() {
    const navigate = useNavigate();
    const username = localStorage.getItem('username') || 'User';

    const handleLogout = () => {
        localStorage.removeItem('authToken');
        localStorage.removeItem('userId');
        localStorage.removeItem('username');
        signalRService.disconnect();
        navigate('/');
    };

    return (
        <div className="home-container">
            <header className="home-header">
                <div className="header-content">
                    <div className="logo-section">
                        <div className="logo">
                            <svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor">
                                <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                            </svg>
                        </div>
                        <h1>ChatAsp</h1>
                    </div>
                    <div className="user-section">
                        <div className="user-greeting">
                            <span className="welcome-text">Welcome back,</span>
                            <span className="username-text">{username}</span>
                        </div>
                        <button onClick={handleLogout} className="logout-btn">
                            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor">
                                <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4M16 17l5-5-5-5M21 12H9" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                            </svg>
                            Logout
                        </button>
                    </div>
                </div>
            </header>

            <main className="home-main">
                <div className="home-layout">
                    <ConversationsSidebar />
                    
                    <div className="main-content">
                        <div className="hero-section">
                            <h2>Connect with People</h2>
                            <p>Start meaningful conversations and stay connected</p>
                        </div>
                        <UserList />
                    </div>
                </div>
            </main>
        </div>
    );
}
