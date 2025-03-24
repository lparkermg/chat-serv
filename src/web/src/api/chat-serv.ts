import { CHATSERV_API_BASE_URL } from "../consts";

export async function CreateRoom(id: string, name: string): Promise<boolean>{
    const resp = await fetch(`${CHATSERV_API_BASE_URL}house/room`,{
        method: "POST",
        body: JSON.stringify({
            id,
            name
        })
    });

    if(resp.status !== 201){
        // Something went wrong because created status wasn't returned.
        return false;
    }

    return true;
}

export async function GetRoomUrl(id: string): Promise<string>{
    const resp = await fetch(`${CHATSERV_API_BASE_URL}house/room/${id}`);
    
    if(resp.status !== 200){
        return "";
    }

    return await resp.text();
}