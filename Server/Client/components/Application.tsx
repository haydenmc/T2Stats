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
                    <Route exact path="/" component={HomePage}/>
                    <Route path="/servers" component={ServersPage}/>
                </div>
            </Router>
        );
    }
}