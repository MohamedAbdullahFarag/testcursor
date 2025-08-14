export enum StatusCodes {
    Ok = 200,
    Created = 201,
    NoContent = 204,
    BadRequest = 400,
    Unauthorized = 401,
    Forbidden = 403,
    NotFound = 404,
    MethodNotAllowed = 405,
    InternalServerError = 500,
    TooManyRequests = 429,
}

export enum THEME {
    LIGHT = 1,
    DARK = 2,
    SYSTEM = 3,
}

export enum UserTypes {
    Anonymous = 'Anonymous',
    Representative = 'InstituteRepresentative',
    Supervisor = 'UniversityManager',
    Candidate = 'UniversityCandidate',
}
export enum DateTimeFormat {
    ArShort = 'DD MMMM YYYY',
    EnShort = 'MMM DD, YYYY',
}
