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
    return <section className="main-screen">
        <form className="room-form" onSubmit={(e) => formSubmitted(e)}>
            <label htmlFor="username" className="form-item">
                <span>Username</span>
                <input type="text" onChange={(e) => setUsername(e.target.value)} />
            </label>
            <label htmlFor="room-id" className="form-item-lg">
                <span>Room Id</span>
                <input type="text" onChange={(e) => setRoomId(e.target.value)} defaultValue={roomId} />
            </label>
            <button type="submit" className="form-item">Connect!</button>
            <button type="reset" className="form-item">Reset</button>
        </form>
    </section>
}

export default MainMenu;