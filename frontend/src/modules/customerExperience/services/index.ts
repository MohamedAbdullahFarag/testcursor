import http from '@/shared/services/http'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import mockData from '../models/constant/data'
import { reportData } from '../models/constant/report'
import { Answer, ApiResponseInterface, MultiLanguageText, Question, ServeyPayload } from '../models/interfaces/CustomerExperience.types'

interface ConditionRule {
    answerId: string
    questionId: string
}

interface SkipQuestionCondition {
    skipQuestionCondition: {
        skipQuestionId: string
    }
    conditionRules: ConditionRule[]
}
const updateTextWithLanguage = (item: Question | Answer, multiLanguageTexts: MultiLanguageText[], lang: string) => {
    const multiText = multiLanguageTexts.find(mlt => mlt.languageItemId === item.id)
    return multiText && multiText.languageCode === lang ? { ...item, text: multiText.itemText ?? '' } : item
}

const getQuestionsForPage = (questions: Question[], pageId: string, multiLanguageTexts: MultiLanguageText[], lang: string) => {
    return questions.filter(question => question.pageId === pageId).map(question => updateTextWithLanguage(question, multiLanguageTexts, lang))
}

const processAnswers = (answers: Answer[] | undefined, multiLanguageTexts: MultiLanguageText[], lang: string) => {
    return answers?.map(answer => updateTextWithLanguage(answer, multiLanguageTexts, lang)) ?? []
}

const SATISFIED_ANSWER_ID = import.meta.env.VITE_SATISFIED_ANSWER_ID
const NOT_SATISFIED_ANSWER_ID = import.meta.env.VITE_NOT_SATISFIED_ANSWER_ID

export const determineIsSatisfied = (questionId: string, skipQuestionConditions?: SkipQuestionCondition[] | null) => {
    // If no skipQuestionConditions exist, return undefined
    if (!skipQuestionConditions?.length) {
        return undefined
    }

    let showSatisfied = false
    let showUnsatisfied = false

    for (const condition of skipQuestionConditions) {
        // Skip if condition is not for this question
        if (condition.skipQuestionCondition.skipQuestionId !== questionId) {
            continue
        }

        // Check condition rules
        for (const rule of condition.conditionRules) {
            if (rule.answerId === SATISFIED_ANSWER_ID) {
                showSatisfied = true
            }
            if (rule.answerId === NOT_SATISFIED_ANSWER_ID) {
                showUnsatisfied = true
            }
        }
    }

    // If we found conditions, return the object with flags
    if (showSatisfied || showUnsatisfied) {
        return { showSatisfied, showUnsatisfied }
    }

    // If we didn't find either specific answerId, return undefined
    return undefined
}

export function useGetQuestions(surveyId: string, lang: 'ar-SA' | 'en-US' = 'ar-SA') {
    const { data, isLoading, isError } = useQuery({
        queryKey: ['surveys', lang, surveyId],
        queryFn: () =>
            new Promise<{ data: ApiResponseInterface }>(resolve => {
                setTimeout(() => {
                    resolve({
                        data: mockData,
                    })
                }, 1000)
            })
                .then(res => {
                    return res.data
                })
                .then((data: ApiResponseInterface) => {
                    // console.log(data.pages, 'data')
                    const pages = data.pages
                    const questions = data.questions
                    const multiLanguageTexts = data.multiLanguageTexts
                    const skipQuestionConditions = data?.skipQuestionConditions
                    const enhancedQuestions = questions.map(question => ({
                        ...question,
                        isSatisfied: determineIsSatisfied(question.id, skipQuestionConditions),
                    }))

                    let paginatedData = []
                    paginatedData = [
                        {
                            ...pages[0],
                            questions: pages.flatMap(page => getQuestionsForPage(enhancedQuestions, page.id, multiLanguageTexts, lang)),
                            answers: processAnswers(data?.answers, multiLanguageTexts, lang),
                            answerTypes: data?.answerTypes ?? [],
                        },
                    ]

                    return {
                        data,
                        paginatedData,
                    }
                })
                .catch(error => {
                    console.error(error)
                }),
        // http
        //     .get<unknown, AxiosResponse<ApiResponseInterface>>(`v1/NGSurvey/ngsurveys/${surveyId}?lang=${lang}`, {
        //         headers: { 'accept-language': `${lang},${lang.split('-')?.[0]};q=0.9` },
        //     })
        //     .then(res => res.data)
        //     .then((data: ApiResponseInterface) => {
        //         const pages = data.pages
        //         const questions = data.questions
        //         const multiLanguageTexts = data.multiLanguageTexts
        //         const skipQuestionConditions = data?.skipQuestionConditions
        //         console.log(questions, 'q')
        //         const enhancedQuestions = questions.map(question => ({
        //             ...question,
        //             isSatisfied: determineIsSatisfied(question.id, skipQuestionConditions),
        //         }))
        //         let paginatedData = []
        //         // if (surveyId === surveiesConfig.DashboardSurvey) {
        //         //     paginatedData = pages.map(page => {
        //         //         return {
        //         //             ...page,
        //         //             questions: getQuestionsForPage(questions, page.id, multiLanguageTexts, lang),
        //         //             answers: processAnswers(data?.answers, multiLanguageTexts, lang),
        //         //             answerTypes: data?.answerTypes ?? [],
        //         //         }
        //         //     })
        //         // } else {
        //         paginatedData = [
        //             {
        //                 ...pages[0],
        //                 questions: pages.flatMap(page => getQuestionsForPage(enhancedQuestions, page.id, multiLanguageTexts, lang)),
        //                 answers: processAnswers(data?.answers, multiLanguageTexts, lang),
        //                 answerTypes: data?.answerTypes ?? [],
        //             },
        //         ]
        //         // }

        //         return {
        //             data,
        //             paginatedData,
        //         }
        //     })
        //     .catch(error => {
        //         console.error(error)
        //     }),
        enabled: !!lang && !!surveyId,
    })

    return {
        data: data?.data as ApiResponseInterface,
        paginatedData: data?.paginatedData ?? [],
        isLoading,
        isError,
        totalPages: data?.paginatedData?.length ?? 1,
    }
}

export const useSubmitSurvey = (surveyId: string, isAuthenticated: boolean, version: number, alias?: string) => {
    const queryClient = useQueryClient()
    return useMutation({
        mutationKey: ['submitSurvey'],
        mutationFn: (payload: ServeyPayload) =>
            http
                .post<unknown>(`/v1/NGSurvey/surveys/${surveyId}/${isAuthenticated ? 'loggedinuserrespondent' : 'respondent'}`, payload)
                .then(res => res?.data)
                .catch(error => {
                    throw error
                }),
        onSuccess: res => {
            if (res && !!alias) {
                queryClient.invalidateQueries({
                    queryKey: [`getServiceRating-${version}-${import.meta.env.MODE}-${alias}`],
                })
                queryClient.invalidateQueries({
                    queryKey: [`getServiceRation-OBPvAFDod6SLDV2lIY6a`],
                })
            }
            return res
        },
    })
}

// const serviceRatingPayload = (version?: number, env?: string, alias?: string) => {
//     if (import.meta.env.MODE === 'production')
//         return {
//             accessPassword: null,
//             timezone: 'Asia/Riyadh',
//             filterId: null,
//             filterCondition: {
//                 conditions: [
//                     {
//                         reportFilterCondition: {
//                             id: 'OGxhM5no12z5tYGQGd4a',
//                             reportFilterId: 'INT13D24DBB622A46D68600FF213FEC3997',
//                             conditionGroupId: 'OGxhM5no12z5tYGQGd3a',
//                             conditionOrder: 1,
//                             internal: true,
//                         },
//                         conditionGroup: {
//                             id: 'OGxhM5no12z5tYGQGd3a',
//                             surveyId: '13D24DBB622A46D68600FF213FEC3997',
//                             internal: true,
//                             logicalOperator: 2,
//                         },
//                         conditionRules: [
//                             {
//                                 id: 'OGxhcJkFqfKvHDqhXSsa',
//                                 conditionRuleTypeId: 21,
//                                 conditionGroupId: 'OGxhM5no12z5tYGQGd3a',
//                                 expressionOperator: 1,
//                                 conditionalOperator: 1,
//                                 expressionDataTypeId: 1,
//                                 groupConditionalOperator: 1,
//                                 internal: true,
//                                 deviceType: 0,
//                                 variableName: null,
//                                 textFilter: null,
//                                 score: null,
//                                 scoreMax: null,
//                             },
//                             {
//                                 id: 'OGxhhVCESoynSTkOr1Za',
//                                 conditionRuleTypeId: 1,
//                                 conditionGroupId: 'OGxhM5no12z5tYGQGd3a',
//                                 expressionOperator: null,
//                                 conditionalOperator: 1,
//                                 expressionDataTypeId: 1,
//                                 groupConditionalOperator: 1,
//                                 internal: true,
//                                 questionId: '814B737F975C4921840F05AAC72CB9E7',
//                                 answerId: '7A60E156BA744EC8854BDD7224F78469',
//                                 textFilter: version,
//                                 variableName: null,
//                                 score: null,
//                                 scoreMax: null,
//                             },
//                             {
//                                 id: 'OGxhlz1HP1VEzvfXf4ya',
//                                 conditionRuleTypeId: 1,
//                                 conditionGroupId: 'OGxhM5no12z5tYGQGd3a',
//                                 expressionOperator: null,
//                                 conditionalOperator: 1,
//                                 expressionDataTypeId: 1,
//                                 groupConditionalOperator: 1,
//                                 internal: true,
//                                 questionId: 'FC69871CBC09415C8760389ABCB326F5',
//                                 answerId: 'F597DB00BDF54076ABAD8B15A07310C7',
//                                 textFilter: env,
//                                 variableName: null,
//                                 score: null,
//                                 scoreMax: null,
//                             },
//                             {
//                                 id: 'OGxhqVFqkdCwKc4kkada',
//                                 conditionRuleTypeId: 1,
//                                 conditionGroupId: 'OGxhM5no12z5tYGQGd3a',
//                                 expressionOperator: null,
//                                 conditionalOperator: 1,
//                                 expressionDataTypeId: 1,
//                                 groupConditionalOperator: 1,
//                                 internal: true,
//                                 questionId: 'C278AC1BF65B43B7A34C1EEF95BDA276',
//                                 answerId: 'D9C0F531138F43AB81C62AD5F3CE29F7',
//                                 textFilter: alias,
//                                 variableName: null,
//                                 score: null,
//                                 scoreMax: null,
//                             },
//                         ],
//                     },
//                 ],
//                 internal: true,
//             },
//         }
//     return {
//         accessPassword: null,
//         timezone: 'Asia/Riyadh',
//         filterId: null,
//         filterCondition: {
//             conditions: [
//                 {
//                     reportFilterCondition: {
//                         id: 'OBZaCC3ahivF4jM97RBa',
//                         reportFilterId: 'INTO9dS6FIacFilIPje6bAa',
//                         conditionGroupId: 'OBZaCC3ahivF4jM97RAa',
//                         conditionOrder: 1,
//                         internal: true,
//                     },
//                     conditionGroup: {
//                         id: 'OBZaCC3ahivF4jM97RAa',
//                         surveyId: 'O9dS6FIacFilIPje6bAa',
//                         internal: true,
//                         logicalOperator: 2,
//                     },
//                     conditionRules: [
//                         {
//                             id: 'OBZvTUq3VNohqfASDEZa',
//                             conditionRuleTypeId: 21,
//                             conditionGroupId: 'OBZaCC3ahivF4jM97RAa',
//                             expressionOperator: 1,
//                             conditionalOperator: 1,
//                             expressionDataTypeId: 1,
//                             groupConditionalOperator: 1,
//                             internal: true,
//                             deviceType: 0,
//                             variableName: null,
//                             textFilter: null,
//                             score: null,
//                             scoreMax: null,
//                         },
//                         {
//                             id: 'OBtj53fxkrUdsbXiGYaa',
//                             conditionRuleTypeId: 1,
//                             conditionGroupId: 'OBZaCC3ahivF4jM97RAa',
//                             expressionOperator: null,
//                             conditionalOperator: 1,
//                             expressionDataTypeId: 1,
//                             groupConditionalOperator: 1,
//                             internal: true,
//                             questionId: 'OBPhaYxAXssimeva1X7a',
//                             variableName: null,
//                             textFilter: version,
//                             score: null,
//                             scoreMax: null,
//                             answerId: 'OBPhaYxAXssimeva1X8a',
//                         },
//                         {
//                             id: 'OBtjBkGQQUwWG2zPkN5a',
//                             conditionRuleTypeId: 1,
//                             conditionGroupId: 'OBZaCC3ahivF4jM97RAa',
//                             expressionOperator: null,
//                             conditionalOperator: 1,
//                             expressionDataTypeId: 1,
//                             groupConditionalOperator: 1,
//                             internal: true,
//                             questionId: 'OBPhpQGM9Lef1Blo86da',
//                             answerId: 'OBPhpQGM9Lef1Blo86ea',
//                             textFilter: env,
//                             variableName: null,
//                             score: null,
//                             scoreMax: null,
//                         },
//                         {
//                             id: 'OBtjXr6Ec7CqPIrkaKIa',
//                             conditionRuleTypeId: 1,
//                             conditionGroupId: 'OBZaCC3ahivF4jM97RAa',
//                             expressionOperator: null,
//                             conditionalOperator: 1,
//                             expressionDataTypeId: 1,
//                             groupConditionalOperator: 1,
//                             internal: true,
//                             questionId: 'OBeQRV5RV47s0JiPzJxa',
//                             answerId: 'OBeQRV5RV47s0JiPzJya',
//                             textFilter: alias,
//                             variableName: null,
//                             score: null,
//                             scoreMax: null,
//                         },
//                     ],
//                 },
//             ],
//             internal: true,
//         },
//     }
// }

export const useGetServiceRating = (id: string, version?: number, env?: string, alias?: string) => {
    // const hasFilter = !!version && !!env && !!alias
    return useQuery({
        queryKey: [`getServiceRation-${id}`, `getServiceRating-${version}-${env}-${alias}`],
        queryFn: () =>
            new Promise(resolve => {
                setTimeout(() => {
                    resolve(reportData)
                }, 1000)
            })
                .then(res => res)
                .then(data => {
                    // console.log(data, 'report')
                    return data
                }),
        // http
        //     .post<object, ServiceRatingResponse>(`v1/NGSurvey/reports/${id}`, hasFilter ? serviceRatingPayload(version, env, alias) : {})
        //     .then(res => res.data)
        //     .catch(error => {
        //         throw error
        //     }),
        enabled: !!id,
    })
}
