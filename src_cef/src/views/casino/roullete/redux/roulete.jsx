const 
SELECT_NEXT_PAGE_LSCUSTOM = 'SELECT_NEXT_PAGE_ROULETE';

export default function roulete(state = 'PAGE_HOME', action) {
  if (action.type === SELECT_NEXT_PAGE_LSCUSTOM) {
    return action.payload;
  }
  return state;
}