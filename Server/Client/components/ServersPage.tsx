import * as React from "react";
import { RouteComponentProps } from "react-router-dom";

// 'HelloProps' describes the shape of props.
// State is never set so we use the 'undefined' type.
export class ServersPage extends React.Component<RouteComponentProps<undefined>, undefined> {
    render() {
        return (
            <header>Servers</header>
        );
    }
}