let OTPTimer: NodeJS.Timeout | undefined = undefined

/**
 *
 * @param start The counter starting point in seconds
 * @param end The counter ending point in seconds
 * @param updateEvery Interval to update the counter, in milliseconds
 * @param onUpdate Function triggered on each counter update
 * @param onCounterEnds Function triggered when the counter ends
 */
export const countDown = ({
    start = 15,
    end = 0,
    updateEvery = 1000,
    onUpdate,
    onCounterEnds,
}: {
    start?: number
    end?: number
    updateEvery?: number
    onUpdate?: (counter: number) => void
    onCounterEnds?: () => void
}) => {
    let counter = start

    if (onUpdate) onUpdate(counter)

    if (isNaN(Number(OTPTimer))) {
        OTPTimer = setInterval(() => {
            if (counter > end) {
                counter -= 1

                if (onUpdate) onUpdate(counter)
            } else if (counter === end) {
                if (onCounterEnds) onCounterEnds()
                clearInterval(OTPTimer)
            }
        }, updateEvery)
    }
}

export const clearOTPTimer = () => {
    if (OTPTimer) {
        clearInterval(OTPTimer)
        OTPTimer = undefined
    }
}
