import { useState } from 'react'
import './App.css'
import MainMenu from './Components/Screens/MainMenu'
import ChatRoom from './Components/Screens/ChatRoom'

function App() {
  const [roomId, setRoomId] = useState<string>("");
  const [username, setUsername] = useState<string>("");

  const [showRoom, setShowRoom] = useState<boolean>(false);

  function mainMenuFormSubmitted(enteredUsername: string, enteredRoomId: string){
    if(enteredUsername === "" || enteredRoomId === "" ){
      return;
    }
    
    setRoomId(enteredRoomId)
    setUsername(enteredUsername)

    setShowRoom(true)
  }

  function onRoomDisconnected(reason: string){
    setRoomId("")
    setUsername("")
    setShowRoom(false)

    console.log({evt: "ROOM_DISCONNECTED", reason})
  }

  

  return (
    <>
      {!showRoom && <MainMenu onFormSubmitted={(un, rid) => mainMenuFormSubmitted(un,rid)} />}
      {showRoom && <ChatRoom roomId={roomId} username={username} onRoomDisconnected={(r) => onRoomDisconnected(r)} onErrorConnectingToRoom={(r => onRoomDisconnected(r))} />}
    </>
  )
}

export default App
