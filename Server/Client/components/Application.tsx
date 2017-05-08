import * as React from "react";
import {
  BrowserRouter as Router,
  Route,
  Link
} from "react-router-dom";

import { HomePage } from "./HomePage";
import { ServersPage } from "./ServersPage";
import { ServerDetailsPage } from "./ServerDetailsPage";

interface ApplicationProps {
    /* This space intentionally left blank */
}

interface ApplicationState {
    headerText: string;
}

export class Application extends React.Component<ApplicationProps, undefined> {
    private setHeaderText = (text: string) => {

    };

    render() {
        return (
            <Router>
                <div>
                    <Route exact path="/" component={HomePage} />
                    <Route exact path="/servers" component={ServersPage} />
                    <Route path="/servers/:ipAddress/:port" component={ServerDetailsPage} />
                    <Route path="/players" component={ServersPage} />
                </div>
            </Router>
        );
    }
}