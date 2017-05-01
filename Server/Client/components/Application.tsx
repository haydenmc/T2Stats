import * as React from "react";
import {
  BrowserRouter as Router,
  Route,
  Link
} from "react-router-dom";

import { HomePage } from "./HomePage";
import { ServersPage } from "./ServersPage";
import { ServerDetailsPage } from "./ServerDetailsPage";

export interface ApplicationProps { }

export class Application extends React.Component<ApplicationProps, undefined> {
    render() {
        return (
            <Router>
                <div>
                    <header><Link to="/"><img src="/img/logo.svg" /></Link></header>
                    <main>
                        <Route exact path="/" component={HomePage} />
                        <Route exact path="/servers" component={ServersPage} />
                        <Route path="/servers/:ipAddress/:port" component={ServerDetailsPage} />
                        <Route path="/players" component={ServersPage} />
                    </main>
                </div>
            </Router>
        );
    }
}