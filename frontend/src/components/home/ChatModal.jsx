import signalRService from "../../services/SignalRService";
import { useEffect, useState, useRef, useCallback } from "react";
import axiosInstance from "../../services/AxiosProvider";
import './ChatModal.css';

export default function ChatModal({userId, username, onClose}) {
    const [messages, setMessages] = useState([]);
    const [newMessage, setNewMessage] = useState('');
    const [roomId, setRoomId] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const messagesEndRef = useRef(null);
    const currentUserId = localStorage.getItem('userId');

    const scrollToBottom = () => {
        messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
    };

    useEffect(() => {
        scrollToBottom();
    }, [messages]);

    const handleReceiveMessage = useCallback((message) => {
        const normalizedMessage = {
            senderId: message.userId,
            content: message.message,
            timestamp: message.timestamp
        };
        setMessages(prev => [...prev, normalizedMessage]);
    }, []);

    const handleUserJoined = useCallback((userId, roomId) => {
    }, []);

    useEffect(() => {
        const initializeChat = async () => {
            try {
                setLoading(true);
                setError('');
                signalRService.off('ReceiveMessage');
                signalRService.off('UserJoined');
                await new Promise(resolve => setTimeout(resolve, 100));
                
                if (!signalRService.isConnected) {
                    const token = localStorage.getItem('authToken');
                    await signalRService.connect(token);
                    await new Promise(resolve => setTimeout(resolve, 200));
                }
                
                const room = await signalRService.invoke('CreateOrJoinChatRoom', parseInt(userId));
                setRoomId(room.id);
                
                signalRService.on('ReceiveMessage', handleReceiveMessage);
                signalRService.on('UserJoined', handleUserJoined);
                
                const messageHistory = await axiosInstance.getMessageHistory(room.id);
                setMessages(messageHistory);

                setLoading(false);
            } catch (err) {
                console.error('Failed to initialize chat:', err);
                setError(err.message);
                setLoading(false);
            }
        };

        initializeChat();

        return () => {
            signalRService.off('ReceiveMessage');
            signalRService.off('UserJoined');
        };
    }, [userId, handleReceiveMessage, handleUserJoined]);

    const handleSendMessage = async (e) => {
        e.preventDefault();
        if (!newMessage.trim() || !roomId) return;

        try {
            await signalRService.invoke('SendMessage', roomId, newMessage);
            // setMessages(prev => [...prev, {
            //     senderId: parseInt(currentUserId),
            //     content: newMessage,
            //     timestamp: new Date().toISOString()
            // }]);
            setNewMessage('');
        } catch (err) {
            console.error('Failed to send message:', err);
            setError(err.message);
        }
    };

    if (loading) {
        return (
            <div className="chat-modal-overlay" onClick={onClose}>
                <div className="chat-modal" onClick={e => e.stopPropagation()}>
                    <div className="chat-loading">
                        <div className="spinner"></div>
                        <p>Connecting to chat...</p>
                    </div>
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="chat-modal-overlay" onClick={onClose}>
                <div className="chat-modal" onClick={e => e.stopPropagation()}>
                    <div className="chat-error">
                        <h3>⚠️ Error connecting to chat</h3>
                        <p>{error}</p>
                        <button onClick={() => window.location.reload()}>Retry</button>
                    </div>
                </div>
            </div>
        );
    }

    return (
        <div className="chat-modal-overlay" onClick={onClose}>
            <div className="chat-modal" onClick={e => e.stopPropagation()}>
                <div className="chat-header">
                    <div className="chat-user-info">
                        <div className="avatar">{username?.charAt(0).toUpperCase()}</div>
                        <div>
                            <h3>{username}</h3>
                            <span className="status online">Online</span>
                        </div>
                    </div>
                    <button className="close-btn" onClick={onClose}>✕</button>
                </div>

                <div className="chat-messages">
                    {messages.length === 0 ? (
                        <div className="no-messages">
                            <p>No messages yet. Start the conversation!</p>
                        </div>
                    ) : (
                        messages.map((msg, index) => (
                            <div 
                                key={index} 
                                className={`message ${msg.senderId == currentUserId ? 'sent' : 'received'}`}
                            >
                                <div className="message-content">
                                    <p>{msg.content}</p>
                                    <span className="timestamp">
                                        {new Date(msg.timestamp).toLocaleTimeString([], { 
                                            hour: '2-digit', 
                                            minute: '2-digit' 
                                        })}
                                    </span>
                                </div>
                            </div>
                        ))
                    )}
                    <div ref={messagesEndRef} />
                </div>

                <form className="chat-input-container" onSubmit={handleSendMessage}>
                    <input
                        type="text"
                        value={newMessage}
                        onChange={(e) => setNewMessage(e.target.value)}
                        placeholder="Type a message..."
                        className="chat-input"
                    />
                    <button type="submit" className="send-btn" disabled={!newMessage.trim()}>
                        <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor">
                            <path d="M22 2L11 13M22 2l-7 20-4-9-9-4 20-7z" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                        </svg>
                    </button>
                </form>
            </div>
        </div>
    );
}
