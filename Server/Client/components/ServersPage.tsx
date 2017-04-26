import * as React from "react";
import { RouteComponentProps } from "react-router-dom";
import { ServerModel }  from "../models/ServerModel";

interface ServersPageState {
    servers: ServerModel[];
}

// 'HelloProps' describes the shape of props.
// State is never set so we use the 'undefined' type.
export class ServersPage extends React.Component<RouteComponentProps<undefined>, ServersPageState> {
    public constructor() {
        super();
        this.state = {
            servers: []
        };
        this.fetchServers();
    }

    private fetchServers(): void {
        var xhr = new XMLHttpRequest();
        xhr.addEventListener("load", (e) => {
            var servers = JSON.parse(xhr.responseText);
            this.setState((prevState, props) => {
                return {
                    ...prevState,
                    servers: servers
                };
            });
        });
        xhr.open("GET", "api/Servers");
        xhr.send();
    }

    public render() {
        return (
            <div>
                {this.state.servers.map(function(object, i){
                    return (
                        <li>
                            {object.name}
                        </li>
                    );
                })}
            </div>
        );
    }
}