import { useMediaQuery } from 'usehooks-ts'

const useGridColumns = () => {
    const isXXLScreen = useMediaQuery('(min-width:1536px)')

    if (isXXLScreen) {
        return 8
    } else {
        return 6
    }
}
export default useGridColumns
