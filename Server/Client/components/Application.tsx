import * as React from "react";
import {
  BrowserRouter as Router,
  Route,
  Link
} from "react-router-dom";

import { HomePage } from "./HomePage";
import { ServersPage } from "./ServersPage";

export interface ApplicationProps { }

export class Application extends React.Component<ApplicationProps, undefined> {
    render() {
        return (
            <Router>
                <div>
                    <header><img src="img/logo.svg" />Stats</header>
                    <main>
                        <Route exact path="/" component={HomePage}/>
                        <Route path="/servers" component={ServersPage}/>
                        <Route path="/players" component={ServersPage}/>
                    </main>
                </div>
            </Router>
        );
    }
}