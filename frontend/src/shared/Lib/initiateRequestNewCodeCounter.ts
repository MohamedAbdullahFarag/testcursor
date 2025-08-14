import { clearOTPTimer, countDown } from './counters'

interface RequestNewCodeCounterProps {
    setCounter: (value: number) => void
    initialTimerCount: number
    timerEnd: number
    onCounterEnds?: () => void
}

export const initiateRequestNewCodeCounter = ({ setCounter, initialTimerCount, timerEnd, onCounterEnds }: RequestNewCodeCounterProps) => {
    clearOTPTimer()

    countDown({
        start: initialTimerCount,
        end: timerEnd,
        updateEvery: 1000,
        onUpdate: setCounter,
        onCounterEnds,
    })
}
