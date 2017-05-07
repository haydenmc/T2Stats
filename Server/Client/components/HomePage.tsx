import * as React from "react";
import { RouteComponentProps, Link } from "react-router-dom";
import { MatchSummaryListItem } from "./MatchSummaryListItem";
import { MatchModel } from "../models/MatchModel";
import { PlayerModel } from "../models/PlayerModel";

// Locally scoped styles
var styles = {
    container: {
        "boxSizing": "border-box",
        "width": "100%",
        "justifyContent": "center",
        "display": "flex",
        "flexWrap": "wrap"
    } as React.CSSProperties,
    summaryPanel: {
        "border": "none",
        "width": "100%",
        "minWidth": "320px",
    } as React.CSSProperties,
    summaryPanelHeader: {
        "fontFamily": "var(--font-family)",
        "fontWeight": "bold",
        "textTransform": "uppercase"
    } as React.CSSProperties,
    summaryPanelListContainer: {
        "display": "flex",
        "flexDirection": "row"
    } as React.CSSProperties,
    matchSummaryListItem: {
        "cursor": "pointer"
    } as React.CSSProperties
};

interface HomePageState {
    matches: MatchModel[];
    players: PlayerModel[];
}

export class HomePage extends React.Component<RouteComponentProps<undefined>, HomePageState> {
    public constructor() {
        super();
        this.state = {
            matches: [],
            players: []
        };
        this.fetchMatches();
        this.fetchPlayers();
    }

    private fetchMatches(): void {
        var xhr = new XMLHttpRequest();
        xhr.addEventListener("load", (e) => {
            var matches = JSON.parse(xhr.responseText);
            this.setState((prevState, props) => {
                return {
                    ...prevState,
                    matches: matches
                };
            });
        });
        xhr.open("GET", "api/Matches/");
        xhr.send();
    }

    private fetchPlayers(): void {
        var xhr = new XMLHttpRequest();
        xhr.addEventListener("load", (e) => {
            var players = JSON.parse(xhr.responseText);
            this.setState((prevState, props) => {
                return {
                    ...prevState,
                    players: players
                };
            });
        });
        xhr.open("GET", "api/Players/");
        xhr.send();
    }

    render() {
        return (
            <div style={styles.container}>
                <div style={styles.summaryPanel}>
                    <header style={styles.summaryPanelHeader}>Recent Matches</header>
                    <div>
                        {this.state.matches.map((object, i) => {
                            return (
                                <MatchSummaryListItem match={object} style={styles.matchSummaryListItem} onClick={() => { this.props.history.push('/matches/' + object.matchId); }} />
                            );
                        })}
                    </div>
                </div>
            </div>
        );
    }
}