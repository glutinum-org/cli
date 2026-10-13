export interface LatLng {
    lat: number;
    lng: number;
}

export type LatLngTuple = [number, number];

export type LatLngExpression = LatLng | LatLngTuple;

export declare function setView(center: LatLngExpression, zoom?: number): void;
