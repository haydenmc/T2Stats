import * as React from "react";
import { Link } from "react-router-dom";

var styles = {
    header: {
        fontFamily: "vars(--font-family)",
        fontSize: "18px",
        fontWeight: "bold",
        textTransform: "uppercase",
        marginBottom: "16px",
        display: "flex",
        alignItems: "center"
    } as React.CSSProperties,
    logo: {
        marginRight: "8px"
    } as React.CSSProperties
};

export class Header extends React.Component<undefined, undefined> {
    public constructor() {
        super();
    }

    render() {
        return (
            <header style={styles.header}>
                <Link to="/"><img src="/img/logo.svg" style={styles.logo} /></Link>
                {this.props.children}
            </header>
        );
    }
}