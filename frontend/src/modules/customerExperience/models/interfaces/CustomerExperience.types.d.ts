export interface Question {
    questionVersionUId: string
    repeatableSectionModeId: number
    maxSections: number
    typeMode: number
    typeCSS: string
    isPanelCreator: boolean
    questionChildCount: number
    excludedFromLink: boolean
    pageId: string
    displayOrder: number
    disabled: boolean
    surveyId: string
    id: string
    layoutModeId: number
    selectionModeId: number
    columnsNumber: number
    text: string
    minSelectionRequired: number
    maxSelectionAllowed: number
    ratingEnabled: boolean
    randomizeAnswers: boolean
    isExtendedFilter: boolean
    hideValidationAsterix: boolean
    calculationModeId: number
    displayQuestionHeader: boolean
    rotateMatrix: boolean
    blockQuestionColumnsNumber: number
    newPanelistAllowed: boolean
    editableAnswers: boolean
    selectionsAsButton: boolean
    defaultReport: boolean
    childQuestionsRandomize: boolean
    conversationReplyType: number
    answerSortOrder: number
    requiredValidationType: number
    linkedQuestionPipeType: number
    linkedQuestionPipeTarget: number
    deleted: boolean
    isSatisfied?: { showSatisfied: boolean; showUnsatisfied: boolean }
}

export interface Answer {
    typeMode: number
    itemCollectionMultipleSelection: boolean
    id: string
    questionId: string
    answerTypeId: string
    typeId: string
    text: string
    displayOrder: number
    required: boolean
    fixedDisplayOrder: boolean
    answerDisplayBehaviorId: number
    syncRefMandatory: boolean
    answerFormId: string
    uniqueAnswer: boolean
    computeSentimentScore: boolean
    extractKeyPhrases: boolean
    readOnly: boolean
    dataClass: number
    charCounter: boolean
    wordCounter: boolean
    deleted: boolean
    requiredValidationType: number
    enableSearchFilter: boolean
    enableSelectAll: boolean
    excludeFromLink: boolean
    hideUntilSelected: boolean
}

export interface AnswerType {
    openProperties: boolean
    readOnly: boolean
    id: string
    typeId: string
    builtIn: boolean
    description: string
    fieldWidth: number
    fieldHeight: number
    typeMode: number
    publicFieldResults: boolean
    versioned: boolean
    privateType: boolean
    typeCSS: string
}

interface Page {
    id: string
    pageNumber: number
    surveyId: string
    type: string
    randomizeQuestions: boolean
    enableSubmitButton: boolean
    displayQuestionNumber: boolean
    useDefaultHeaderFooter: boolean
    disableResponseTimer: boolean
    disabled: boolean
    displayAllQuestions: boolean
    highlightRespondentReportAnswers: boolean
    useDefaultPageNavigator: boolean
    deadEnd: boolean
}

export interface MultiLanguageText {
    multiLanguageTextUId: string
    languageItemId: string //question ID
    itemText: string
    languageCode: string
}

export interface SkippedQuestion {
    skipQuestionCondition: SkipQuestionCondition
    conditionGroup: ConditionGroup
    conditionRules: ConditionRule[]
}

export interface SkipQuestionCondition {
    surveyId: string
    id: string
    conditionGroupId: string
    skipQuestionId: string
    skipDisplayModeId: number
    conditionOrder: number
}

export interface ConditionGroup {
    id: string
    surveyId: string
    logicalOperator: number
}

export interface ConditionRule {
    id: string
    conditionGroupId: string
    questionId: string
    answerId: string
    conditionRuleTypeId: number
    conditionalOperator: number
    groupConditionalOperator: number
    expressionDataTypeId: number
}

export interface ApiResponseInterface {
    answers: Answer[]
    questions: Question[]
    answerTypes: { answerType: AnswerType }[]
    pages: Page[]
    multiLanguageTexts: MultiLanguageText[]
    skipQuestionConditions: SkippedQuestion[]
}
export interface PaginatedCustomerExpData extends Page {
    answers: Answer[]
    questions: Question[]
    answerTypes: { answerType: AnswerType }[]
}

export interface PayloadAnswer {
    respondentId: string
    answerId: string | undefined
    value: string | null
    sectionNumber: number
    disabled: boolean
}
export interface ServeyPayload {
    respondent: {
        respondent: {
            id: string
            surveyId: string
            resumeUId: null
            startDate: string
            iPSource: null
            changeUID: null
            languageCode: string
            validated: boolean
            platform: string
            userAgent: string
            mobileOS: boolean
            platformOS: number
            resumeQuestionId: string
            resumePageId: string
            timeZone: string
        }
        answers: PayloadAnswer[]
        dataAttributes: null
        files: []
        poolQuestions: []
        querystring: string
    }
    securityAuthorization: []
    Captcha: {
        token: string
    }
}

export interface ServiceRatingResponse {
    data: {
        report: Report
        reportItems: ReportItem[]
    }
}

export interface Report {
    passwordRequired: boolean
    id: string
    surveyId: string
    reportName: string
    publicReport: boolean
    defaultReport: boolean
    publicResultFiltering: boolean
    filterLanguage: string
    ownerUserId: string
}

export interface ReportItem {
    reportItem: ReportItem2
    questionIds: string[]
    chartsData: ChartsDaum[]
}

export interface ReportItem2 {
    typeMode: number
    id: string
    reportId: string
    reportItemTypeId: string
    title: string
    displayOrder: number
    pageNumber: number
    filterLanguage: string
}
