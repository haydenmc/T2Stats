import { ServerModel } from "./ServerModel";

export interface MatchModel {
    matchId: string;
    server: ServerModel;
    mapName: string;
    gameType: string;
    startTime: Date;
    duration: string;
}