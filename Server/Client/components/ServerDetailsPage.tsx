import * as React from "react";
import { RouteComponentProps, withRouter } from "react-router-dom";
import { ServerModel }  from "../models/ServerModel";

interface ServerDetailsPageProps {
    ipAddress: string;
    port: string;
}

interface ServerDetailsPageState {
    server: ServerModel;
    matches: any[];
}

// 'HelloProps' describes the shape of props.
// State is never set so we use the 'undefined' type.
class BaseServerDetailsPage extends React.Component<RouteComponentProps<ServerDetailsPageProps>, ServerDetailsPageState> {
    public constructor(props: RouteComponentProps<ServerDetailsPageProps>) {
        super(props);
        this.state = {
            server: {
                serverId: "",
                ipAddress: this.props.match.params.ipAddress,
                port: this.props.match.params.port,
                name: ""
            },
            matches: []
        };
        this.fetchServerDetails();
    }

    private fetchServerDetails(): void {
        // TODO
        // var xhr = new XMLHttpRequest();
        // xhr.addEventListener("load", (e) => {
        //     var servers = JSON.parse(xhr.responseText);
        //     this.setState((prevState, props) => {
        //         return {
        //             ...prevState,
        //             servers: servers
        //         };
        //     });
        // });
        // xhr.open("GET", "api/Servers");
        // xhr.send();
    }

    public render() {
        return (
            <div>
                <header>{this.state.server.name}</header>
                <p>{this.state.server.ipAddress}:{this.state.server.port}</p>
            </div>
        );
    }
}

export var ServerDetailsPage = withRouter(BaseServerDetailsPage);