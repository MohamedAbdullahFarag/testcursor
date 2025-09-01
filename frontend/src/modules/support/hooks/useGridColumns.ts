import { useMemo } from 'react'
import { useMediaQuery } from 'usehooks-ts'

const useGridColumns = () => {
    const isXXLScreen = useMediaQuery('(min-width:1536px)')

    // Memoize the result to prevent unnecessary re-renders
    const result = useMemo(() => {
        const columns = isXXLScreen ? 8 : 6
        console.log(`useGridColumns: isXXLScreen=${isXXLScreen}, returning=${columns}`)
        return columns
    }, [isXXLScreen])

    return result
}
export default useGridColumns
