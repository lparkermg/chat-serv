import { useState } from "react";

interface MainMenuProps {
    onFormSubmitted(username: string, roomId: string): void
}

function MainMenu({onFormSubmitted}: MainMenuProps){

    const [username, setUsername] = useState<string>("");
    const [roomId, setRoomId] = useState<string>("global");
    function formSubmitted(e:React.FormEvent){
        e.preventDefault()

        onFormSubmitted(username, roomId);
    }
    return <section>
        <form onSubmit={(e) => formSubmitted(e)}>
            <label htmlFor="username">
                <span>Username</span>
                <input type="text" onChange={(e) => setUsername(e.target.value)} />
            </label>
            <label htmlFor="room-id">
                <span>Room Id</span>
                <input type="text" onChange={(e) => setRoomId(e.target.value)} defaultValue={roomId} />
            </label>
            <button type="submit">Connect!</button>
            <button type="reset">Reset</button>
        </form>
    </section>
}

export default MainMenu;