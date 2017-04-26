import * as React from "react";
import { RouteComponentProps, withRouter } from "react-router-dom";
import { ServerModel }  from "../models/ServerModel";

interface ServersPageProps {
    history: History;
}

interface ServersPageState {
    servers: ServerModel[];
}

// 'HelloProps' describes the shape of props.
// State is never set so we use the 'undefined' type.
class BaseServersPage extends React.Component<RouteComponentProps<undefined>, ServersPageState> {
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
                {this.state.servers.map((object, i) => {
                    return (
                        <li onClick={() => { this.props.history.push('/servers/' + object.ipAddress + '/' + object.port); }}>
                            {object.name}
                        </li>
                    );
                })}
            </div>
        );
    }
}

export var ServersPage = withRouter(BaseServersPage);