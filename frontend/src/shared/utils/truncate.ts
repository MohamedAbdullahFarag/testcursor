import { isArabicWord } from './isArabicWord'

export function __arTruncateStr(str: string, n: number) {
    if (!isArabicWord(str)) return str.slice(0, n - 1).padStart(n + 2, '...')
    return str.slice(0, n - 1).padEnd(n + 2, '...')
}

export function __enTruncateStr(str: string, n: number) {
    if (!isArabicWord(str)) return str.slice(0, n - 1).padEnd(n + 2, '...')
    return str.slice(0, n - 1).padStart(n + 2, '...')
}

export function truncateStr(str: string, n: number) {
    if (typeof str === 'string') {
        if (str.length > n) {
            if (document.documentElement.dir !== 'rtl') {
                return __enTruncateStr(str, n)
            }
            return __arTruncateStr(str, n)
        }
        return str
    }
    return str
}
