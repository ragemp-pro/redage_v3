import { executeClientToGroup} from "api/rage";
import { currentPage, selectNumber } from "@/views/player/hudevo/phonenew/stores";

export const onCall = (number) => {
    selectNumber.set(number);
    executeClientToGroup ("call", number)
    currentPage.set ("callView");
}

export const onMessage = (number) => {
    selectNumber.set(number);
    currentPage.set ("messages");
}

export const onInputFocus = () => {
    executeClientToGroup ('inputFocus', true);
}

export const onInputBlur = () => {
    executeClientToGroup ('inputFocus', false);
}