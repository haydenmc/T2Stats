import * as React from "react";
import { RouteComponentProps, withRouter } from "react-router-dom";
import { MatchModel }  from "../models/MatchModel";

interface MatchSummaryListItemProps {
    match: MatchModel;
    onClick?: () => void;
    style?: React.CSSProperties;
}

// Locally scoped styles
var styles = {
    container: {
        width: "300px",
        padding: "4px 4px 4px 0"
    } as React.CSSProperties,
    matchTitle: {
        fontFamily: "var(--font-family)",
        fontWeight: 900,
        textTransform: "uppercase",
        fontSize: "32px",
        lineHeight: 1,
        whiteSpace: "nowrap",
        overflow: "hidden",
        textOverflow: "ellipsis" 
    } as React.CSSProperties,
    matchSubtitle: {
        fontFamily: "var(--font-family)",
        fontWeight: 100,
        textTransform: "lowercase",
        fontSize: "18px",
        paddingBottom: "4px",
        lineHeight: 1
    } as React.CSSProperties,
    matchDetails: {
        fontFamily: "var(--font-family)",
        fontWeight: 400,
        fontSize: "12px"
    } as React.CSSProperties
};

export class MatchSummaryListItem extends React.Component<MatchSummaryListItemProps, undefined> {
    public constructor(props: MatchSummaryListItemProps) {
        super(props);
        // Override styles from props
        for (var style in props.style) {
            styles.container[style] = props.style[style];
        }
    }

    public render() {
        return (
            <div style={styles.container} onClick={this.props.onClick}>
                <div style={styles.matchTitle}>{this.props.match.mapName}</div>
                <div style={styles.matchSubtitle}>{this.props.match.gameType}</div>
                <div style={styles.matchDetails}>{this.props.match.server.name}</div>
                <div style={styles.matchDetails}>{this.props.match.startTime}</div>
            </div>
        );
    }
}