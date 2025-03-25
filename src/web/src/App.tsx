import { useState } from 'react'
import './App.css'
import MainMenu from './Components/Screens/MainMenu'
import ChatRoom from './Components/Screens/ChatRoom'
import ErrorBar from './Components/Generic/ErrorBar';

function App() {
  const [roomId, setRoomId] = useState<string>("");
  const [username, setUsername] = useState<string>("");

  const [showRoom, setShowRoom] = useState<boolean>(false);

  const [errorMessage, setErrorMessage] = useState<string | undefined>(undefined);

  function mainMenuFormSubmitted(enteredUsername: string, enteredRoomId: string){
    if(enteredUsername === "" || enteredRoomId === "" ){
      return;
    }
    
    setRoomId(enteredRoomId)
    setUsername(enteredUsername)

    setShowRoom(true)
    setErrorMessage(undefined)
  }

  function onRoomDisconnected(reason: string){
    setRoomId("")
    setUsername("")
    setShowRoom(false)

    setErrorMessage(reason)
  }

  return (
    <>
      {errorMessage !== undefined && <ErrorBar message={errorMessage}/>}
      <h1>Chat Serv - Demo!</h1>
      {!showRoom && <MainMenu onFormSubmitted={(un, rid) => mainMenuFormSubmitted(un,rid)} />}
      {showRoom && <ChatRoom roomId={roomId} username={username} onRoomDisconnected={(r) => onRoomDisconnected(r)} onErrorConnectingToRoom={(r => onRoomDisconnected(r))} />}
    </>
  )
}

export default App
