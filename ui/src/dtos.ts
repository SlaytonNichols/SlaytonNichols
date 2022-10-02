/* Options:
Date: 2022-10-01 23:28:34
Version: 6.21
Tip: To override a DTO option, remove "//" prefix before updating
BaseUrl: https://localhost:5001

//GlobalNamespace: 
MakePropertiesOptional: True
//AddServiceStackTypes: True
//AddResponseStatus: False
//AddImplicitVersion: 
//AddDescriptionAsComments: True
//IncludeTypes: 
//ExcludeTypes: 
//DefaultImports: 
*/


export interface IReturn<T>
{
    createResponse(): T;
}

export interface IReturnVoid
{
    createResponse(): void;
}

export interface IHasSessionId
{
    sessionId?: string;
}

export interface IHasBearerToken
{
    bearerToken?: string;
}

export interface IPost
{
}

export interface ICreateDb<Table>
{
}

export interface IPatchDb<Table>
{
}

// @DataContract
export class QueryBase
{
    // @DataMember(Order=1)
    public skip?: number;

    // @DataMember(Order=2)
    public take?: number;

    // @DataMember(Order=3)
    public orderBy?: string;

    // @DataMember(Order=4)
    public orderByDesc?: string;

    // @DataMember(Order=5)
    public include?: string;

    // @DataMember(Order=6)
    public fields?: string;

    // @DataMember(Order=7)
    public meta?: { [index: string]: string; };

    public constructor(init?: Partial<QueryBase>) { (Object as any).assign(this, init); }
}

export class QueryDb<T> extends QueryBase
{

    public constructor(init?: Partial<QueryDb<T>>) { super(init); (Object as any).assign(this, init); }
}

// @DataContract
export class AuditBase
{
    // @DataMember(Order=1)
    public createdDate?: string;

    // @DataMember(Order=2)
    // @Required()
    public createdBy?: string;

    // @DataMember(Order=3)
    public modifiedDate?: string;

    // @DataMember(Order=4)
    // @Required()
    public modifiedBy?: string;

    // @DataMember(Order=5)
    public deletedDate?: string;

    // @DataMember(Order=6)
    public deletedBy?: string;

    public constructor(init?: Partial<AuditBase>) { (Object as any).assign(this, init); }
}

/**
* Blog post
*/
export class Post extends AuditBase
{
    public id?: number;
    public mdText?: string;
    public name?: string;
    public path?: string;

    public constructor(init?: Partial<Post>) { super(init); (Object as any).assign(this, init); }
}

// @DataContract
export class ResponseError
{
    // @DataMember(Order=1)
    public errorCode?: string;

    // @DataMember(Order=2)
    public fieldName?: string;

    // @DataMember(Order=3)
    public message?: string;

    // @DataMember(Order=4)
    public meta?: { [index: string]: string; };

    public constructor(init?: Partial<ResponseError>) { (Object as any).assign(this, init); }
}

// @DataContract
export class ResponseStatus
{
    // @DataMember(Order=1)
    public errorCode?: string;

    // @DataMember(Order=2)
    public message?: string;

    // @DataMember(Order=3)
    public stackTrace?: string;

    // @DataMember(Order=4)
    public errors?: ResponseError[];

    // @DataMember(Order=5)
    public meta?: { [index: string]: string; };

    public constructor(init?: Partial<ResponseStatus>) { (Object as any).assign(this, init); }
}

// @DataContract
export class QueryResponse<T>
{
    // @DataMember(Order=1)
    public offset?: number;

    // @DataMember(Order=2)
    public total?: number;

    // @DataMember(Order=3)
    public results?: T[];

    // @DataMember(Order=4)
    public meta?: { [index: string]: string; };

    // @DataMember(Order=5)
    public responseStatus?: ResponseStatus;

    public constructor(init?: Partial<QueryResponse<T>>) { (Object as any).assign(this, init); }
}

// @DataContract
export class AuthenticateResponse implements IHasSessionId, IHasBearerToken
{
    // @DataMember(Order=1)
    public userId?: string;

    // @DataMember(Order=2)
    public sessionId?: string;

    // @DataMember(Order=3)
    public userName?: string;

    // @DataMember(Order=4)
    public displayName?: string;

    // @DataMember(Order=5)
    public referrerUrl?: string;

    // @DataMember(Order=6)
    public bearerToken?: string;

    // @DataMember(Order=7)
    public refreshToken?: string;

    // @DataMember(Order=8)
    public profileUrl?: string;

    // @DataMember(Order=9)
    public roles?: string[];

    // @DataMember(Order=10)
    public permissions?: string[];

    // @DataMember(Order=11)
    public responseStatus?: ResponseStatus;

    // @DataMember(Order=12)
    public meta?: { [index: string]: string; };

    public constructor(init?: Partial<AuthenticateResponse>) { (Object as any).assign(this, init); }
}

// @DataContract
export class AssignRolesResponse
{
    // @DataMember(Order=1)
    public allRoles?: string[];

    // @DataMember(Order=2)
    public allPermissions?: string[];

    // @DataMember(Order=3)
    public meta?: { [index: string]: string; };

    // @DataMember(Order=4)
    public responseStatus?: ResponseStatus;

    public constructor(init?: Partial<AssignRolesResponse>) { (Object as any).assign(this, init); }
}

// @DataContract
export class UnAssignRolesResponse
{
    // @DataMember(Order=1)
    public allRoles?: string[];

    // @DataMember(Order=2)
    public allPermissions?: string[];

    // @DataMember(Order=3)
    public meta?: { [index: string]: string; };

    // @DataMember(Order=4)
    public responseStatus?: ResponseStatus;

    public constructor(init?: Partial<UnAssignRolesResponse>) { (Object as any).assign(this, init); }
}

// @DataContract
export class RegisterResponse implements IHasSessionId, IHasBearerToken
{
    // @DataMember(Order=1)
    public userId?: string;

    // @DataMember(Order=2)
    public sessionId?: string;

    // @DataMember(Order=3)
    public userName?: string;

    // @DataMember(Order=4)
    public referrerUrl?: string;

    // @DataMember(Order=5)
    public bearerToken?: string;

    // @DataMember(Order=6)
    public refreshToken?: string;

    // @DataMember(Order=7)
    public roles?: string[];

    // @DataMember(Order=8)
    public permissions?: string[];

    // @DataMember(Order=9)
    public responseStatus?: ResponseStatus;

    // @DataMember(Order=10)
    public meta?: { [index: string]: string; };

    public constructor(init?: Partial<RegisterResponse>) { (Object as any).assign(this, init); }
}

// @DataContract
export class IdResponse
{
    // @DataMember(Order=1)
    public id?: string;

    // @DataMember(Order=2)
    public responseStatus?: ResponseStatus;

    public constructor(init?: Partial<IdResponse>) { (Object as any).assign(this, init); }
}

/**
* Find posts
*/
// @Route("/posts", "GET")
// @Route("/posts/{Path}", "GET")
export class QueryPosts extends QueryDb<Post> implements IReturn<QueryResponse<Post>>
{
    public path?: string;

    public constructor(init?: Partial<QueryPosts>) { super(init); (Object as any).assign(this, init); }
    public getTypeName() { return 'QueryPosts'; }
    public getMethod() { return 'GET'; }
    public createResponse() { return new QueryResponse<Post>(); }
}

/**
* Sign In
*/
// @Route("/auth", "OPTIONS,GET,POST,DELETE")
// @Route("/auth/{provider}", "OPTIONS,GET,POST,DELETE")
// @Api(Description="Sign In")
// @DataContract
export class Authenticate implements IReturn<AuthenticateResponse>, IPost
{
    /**
    * AuthProvider, e.g. credentials
    */
    // @DataMember(Order=1)
    public provider?: string;

    // @DataMember(Order=2)
    public state?: string;

    // @DataMember(Order=3)
    public oauth_token?: string;

    // @DataMember(Order=4)
    public oauth_verifier?: string;

    // @DataMember(Order=5)
    public userName?: string;

    // @DataMember(Order=6)
    public password?: string;

    // @DataMember(Order=7)
    public rememberMe?: boolean;

    // @DataMember(Order=9)
    public errorView?: string;

    // @DataMember(Order=10)
    public nonce?: string;

    // @DataMember(Order=11)
    public uri?: string;

    // @DataMember(Order=12)
    public response?: string;

    // @DataMember(Order=13)
    public qop?: string;

    // @DataMember(Order=14)
    public nc?: string;

    // @DataMember(Order=15)
    public cnonce?: string;

    // @DataMember(Order=17)
    public accessToken?: string;

    // @DataMember(Order=18)
    public accessTokenSecret?: string;

    // @DataMember(Order=19)
    public scope?: string;

    // @DataMember(Order=20)
    public meta?: { [index: string]: string; };

    public constructor(init?: Partial<Authenticate>) { (Object as any).assign(this, init); }
    public getTypeName() { return 'Authenticate'; }
    public getMethod() { return 'POST'; }
    public createResponse() { return new AuthenticateResponse(); }
}

// @Route("/assignroles", "POST")
// @DataContract
export class AssignRoles implements IReturn<AssignRolesResponse>, IPost
{
    // @DataMember(Order=1)
    public userName?: string;

    // @DataMember(Order=2)
    public permissions?: string[];

    // @DataMember(Order=3)
    public roles?: string[];

    // @DataMember(Order=4)
    public meta?: { [index: string]: string; };

    public constructor(init?: Partial<AssignRoles>) { (Object as any).assign(this, init); }
    public getTypeName() { return 'AssignRoles'; }
    public getMethod() { return 'POST'; }
    public createResponse() { return new AssignRolesResponse(); }
}

// @Route("/unassignroles", "POST")
// @DataContract
export class UnAssignRoles implements IReturn<UnAssignRolesResponse>, IPost
{
    // @DataMember(Order=1)
    public userName?: string;

    // @DataMember(Order=2)
    public permissions?: string[];

    // @DataMember(Order=3)
    public roles?: string[];

    // @DataMember(Order=4)
    public meta?: { [index: string]: string; };

    public constructor(init?: Partial<UnAssignRoles>) { (Object as any).assign(this, init); }
    public getTypeName() { return 'UnAssignRoles'; }
    public getMethod() { return 'POST'; }
    public createResponse() { return new UnAssignRolesResponse(); }
}

/**
* Sign Up
*/
// @Route("/register", "PUT,POST")
// @Api(Description="Sign Up")
// @DataContract
export class Register implements IReturn<RegisterResponse>, IPost
{
    // @DataMember(Order=1)
    public userName?: string;

    // @DataMember(Order=2)
    public firstName?: string;

    // @DataMember(Order=3)
    public lastName?: string;

    // @DataMember(Order=4)
    public displayName?: string;

    // @DataMember(Order=5)
    public email?: string;

    // @DataMember(Order=6)
    public password?: string;

    // @DataMember(Order=7)
    public confirmPassword?: string;

    // @DataMember(Order=8)
    public autoLogin?: boolean;

    // @DataMember(Order=10)
    public errorView?: string;

    // @DataMember(Order=11)
    public meta?: { [index: string]: string; };

    public constructor(init?: Partial<Register>) { (Object as any).assign(this, init); }
    public getTypeName() { return 'Register'; }
    public getMethod() { return 'POST'; }
    public createResponse() { return new RegisterResponse(); }
}

/**
* Create a new post
*/
// @Route("/posts", "POST")
// @ValidateRequest(Validator="HasRole(`Admin`)")
export class CreatePost implements IReturn<IdResponse>, ICreateDb<Post>
{
    public id?: number;
    public mdText?: string;
    public name?: string;
    public path?: string;

    public constructor(init?: Partial<CreatePost>) { (Object as any).assign(this, init); }
    public getTypeName() { return 'CreatePost'; }
    public getMethod() { return 'POST'; }
    public createResponse() { return new IdResponse(); }
}

/**
* Update an existing post
*/
// @Route("/posts/{Id}", "PATCH")
// @ValidateRequest(Validator="HasRole(`Admin`)")
export class UpdatePost implements IReturn<IdResponse>, IPatchDb<Post>
{
    public id?: number;
    public mdText?: string;
    public name?: string;
    public path?: string;

    public constructor(init?: Partial<UpdatePost>) { (Object as any).assign(this, init); }
    public getTypeName() { return 'UpdatePost'; }
    public getMethod() { return 'PATCH'; }
    public createResponse() { return new IdResponse(); }
}

