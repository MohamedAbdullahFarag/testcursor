/**
 * Checks if a string contains any Arabic characters.
 * @param str - The string to check for Arabic characters
 * @returns boolean - True if the string contains at least one Arabic character, false otherwise
 */
export function isArabicWord(str: string): boolean {
    return /[\u0600-\u06FF]/.exec(str) !== null
}
