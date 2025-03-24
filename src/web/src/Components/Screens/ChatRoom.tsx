import { useEffect, useState } from "react";
import * as api from '../../api/chat-serv';

interface ChatRoomProps{
    roomId: string;
    username: string;
    onRoomDisconnected(reason: string): void;
    onErrorConnectingToRoom(message: string): void;
}

interface BNTMessageModel{
    sender: string;
    content: number;
}

function ChatRoom({roomId, username, onRoomDisconnected, onErrorConnectingToRoom}: ChatRoomProps){
    const [roomConnection, setRoomConnection] = useState<WebSocket | undefined>(undefined);
    const [messages, setMessages] = useState<string[]>([]);

    useEffect(() => {
        connectToRoom()
    })

    async function connectToRoom()
    {
        if (roomConnection){
            console.warn("room already connected");
            return;
        }

        try{
            const roomUrl = await api.GetRoomUrl(roomId)
            const connection = new WebSocket(roomUrl, "string");
            connection.onopen = (ev: Event) => console.log("Connection openned", {ev});
            connection.onmessage = messageRecieved
            connection.onclose = (ev: CloseEvent) => {
                console.log("Connection closed", {ev});
                onRoomDisconnected(JSON.stringify(ev));
            }
            connection.onerror = (ev: Event) => console.log("Connection error", {ev})
            setRoomConnection(connection);
        }
        catch(err: any){
            console.error({err, msg: err.message});
            onErrorConnectingToRoom(err.message);
        }
    }

    function disconnectFromRoom()
    {
        try{
            if(roomConnection === undefined){
                console.warn("Already disconnecting or disconnected.")
                return;
            }

            roomConnection.close();
        }
        catch(err: any){
            console.error({err, msg: "There was a problem disconnecting from this room"});
            
        }
    }

    function sendMessage(messageId: number){
        if (!roomConnection)
        {
            console.warn("Room is not connected... something has gone wrong.");
            return;
        }

        const message: BNTMessageModel = {
            sender: username,
            content: messageId,
        }
        const parsedMessage = JSON.stringify(message);
        const encoder = new TextEncoder()
        roomConnection.send(encoder.encode(parsedMessage));
    }

    function messageRecieved(ev: MessageEvent)
    {
        console.log("Message received", {ev});
        const updatedMessages = [...messages];
        updatedMessages.push(ev.data as string);
        setMessages(updatedMessages)
    }

    const processedMessages = messages.map((v) => <li>{v}</li>)

    return <section>
        <nav>
            <div>
                <button onClick={() => sendMessage(0)}>Hello!</button>
                <button onClick={() => sendMessage(1)}>How are you?</button>
                <button onClick={() => sendMessage(2)}>I'm good!</button>
                <button onClick={() => sendMessage(3)}>Not great...</button>
            </div>
            <div>
                <button onClick={() => disconnectFromRoom()}>Leave</button>
            </div>
        </nav>
        <ul>
            {processedMessages}
        </ul>
    </section>
}

export default ChatRoom;