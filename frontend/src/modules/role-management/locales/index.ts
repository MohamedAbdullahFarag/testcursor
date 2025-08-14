import { strings as enStrings } from './en'
import { strings as arStrings } from './ar'

export const strings = document.documentElement.lang === 'ar' ? arStrings : enStrings
