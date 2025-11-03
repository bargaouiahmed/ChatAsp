import { HubConnectionBuilder, LogLevel, HubConnectionState } from "@microsoft/signalr";

class SignalRService {
    constructor() {
        this.connection = null;
    }

    async connect(token) {
        if (this.connection && this.connection.state === HubConnectionState.Connected) {
            return;
        }

        try {
            this.connection = new HubConnectionBuilder()
                .withUrl(import.meta.env.VITE_SIGNALR_HUB_URL || 'http://localhost:5000/hub/chat', {
                    accessTokenFactory: () => token
                })
                .configureLogging(LogLevel.Information)
                .withAutomaticReconnect([0, 0, 5000, 10000, 15000])
                .build();

            await this.connection.start();
        } catch (err) {
            console.error('SignalR Connection Error:', err);
            throw err;
        }
    }

    async disconnect() {
        if (this.connection) {
            try {
                await this.connection.stop();
                this.connection = null;
            } catch (err) {
                console.error('SignalR Disconnection Error:', err);
            }
        }
    }

    on(eventName, callback) {
        if (this.connection) {
            this.connection.on(eventName, callback);
        }
    }

    off(eventName) {
        if (this.connection) {
            this.connection.off(eventName);
        }
    }

    async invoke(methodName, ...args) {
        if (!this.connection || this.connection.state !== HubConnectionState.Connected) {
            throw new Error('SignalR not connected');
        }
        try {
            return await this.connection.invoke(methodName, ...args);
        } catch (err) {
            console.error(`Error invoking ${methodName}:`, err);
            throw err;
        }
    }

    getConnection() {
        return this.connection;
    }

    get isConnected() {
        return this.connection && this.connection.state === HubConnectionState.Connected;
    }
}

const signalRService = new SignalRService();
export default signalRService;