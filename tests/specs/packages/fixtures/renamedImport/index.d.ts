import { Position as VPosition, Range as VRange } from 'dep-lib';
import { Marker } from './marker';

export interface Provider {
    provide(position: VPosition): VRange | undefined;
    mark(marker: Marker): void;
}
