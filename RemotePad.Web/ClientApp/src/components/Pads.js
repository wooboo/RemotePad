import React, { useEffect, useState } from "react";
import authService from "./api-authorization/AuthorizeService";
const signalR = require('@microsoft/signalr');

export const Pads = () => {
    const [connection, setConnection] = useState(null);
    const [hosts, setHosts] = useState({});
    useEffect(() => {
        const hubConnection = new signalR.HubConnectionBuilder()
            .withUrl("/padshub", {
                accessTokenFactory: () => authService.getAccessToken()
            })
            .withAutomaticReconnect()
            .build();

        hubConnection
            .start()
            .then(() => {
                console.log('Connection started!');
                setConnection(hubConnection);
            })
            .catch(err => console.log('Error while establishing connection :('));

    }, [])
    useEffect(() => {
        if (connection) {
            connection.invoke('RequestHosts')
                .catch(err => console.error(err));
        }
    }, [connection]);
    useEffect(() => {
        if (connection) {
            connection.on("receiveHost", (id, host) => {
                setHosts({ ...hosts, [id]: host });
            });
        }
    }, [connection, hosts])
    return <ul>
        {JSON.stringify(hosts)
        }
        </ul>;
};
