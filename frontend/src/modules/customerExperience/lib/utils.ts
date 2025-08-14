import { Answer, Question } from '../models/interfaces/CustomerExperience.types'

export function structureAnswerByQuestionId(answers: Answer[] = []): Record<string, Answer[]> {
    return answers.reduce((acc: Record<string, Answer[]>, answer) => {
        if (!acc[answer.questionId]) {
            acc[answer.questionId] = []
        }
        acc[answer.questionId].push(answer)
        return acc
    }, {})
}

export function sortQuestionsWithAnswers(questions: Question[] = [], answers: Answer[] = []) {
    const answersByQuestionId = structureAnswerByQuestionId(answers || [])

    return questions.toSorted((a, b) => a.displayOrder - b.displayOrder).map(q => ({ question: { ...q }, answers: answersByQuestionId[q.id] || [] }))
}

export function processAnswers(
    respondentId: string,
    formAnswers: Record<string, string | string[]> = {},
    questions: Question[] = [],
    answers: Answer[] = [],
) {
    const payloadAnswers = []
    const sortedQuestionsWithAnswers = sortQuestionsWithAnswers(questions, answers)
    for (const [questionId, answer] of Object.entries(formAnswers)) {
        const question = sortedQuestionsWithAnswers.find(q => q?.question?.id === questionId)

        if (question) {
            const singleAnswer = question?.question?.typeCSS === 'ngs-question-single' && question?.answers?.length === 1
            const hiddenQuestion = question?.question?.typeCSS === 'ngs-question-hidden'

            if (singleAnswer || hiddenQuestion) {
                const ans = question.answers.find(ans => ans.questionId === questionId)
                payloadAnswers.push({
                    respondentId,
                    answerId: ans?.id,
                    value: answer,
                    sectionNumber: 0,
                    disabled: false,
                })
            } else if (Array.isArray(answer)) {
                payloadAnswers.push(
                    ...answer.map(id => ({
                        respondentId,
                        answerId: id,
                        value: null,
                        sectionNumber: 0,
                        disabled: false,
                    })),
                )
            } else {
                payloadAnswers.push({
                    respondentId,
                    answerId: answer,
                    value: null,
                    sectionNumber: 0,
                    disabled: false,
                })
            }
        }
    }

    return payloadAnswers
}
