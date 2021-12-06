module Functions.Expressions

open System


type Expr =
    | Val of float
    | Var of string
    | Add of Expr * Expr
    | Sub of Expr * Expr
    | Mul of Expr * Expr
    | Div of Expr * Expr
    | Pow of Expr * Expr
    | Ln of Expr
    | Cos of Expr
    | Sin of Expr
    | Exp of Expr


let rec diff f x =
    match f with
    | Var y when x = y -> Val 1.0
    | Val _
    | Var _ -> Val 0.0
    | Add (f, g) -> Add(diff f x, diff g x)
    | Sub (f, g) -> Sub(diff f x, diff g x)
    | Mul (f, g) -> Add(Mul(f, diff g x), Mul(g, diff f x))
    | Div (f, g) -> Div(Sub(Mul(diff f x, g), Mul(diff g x, f)), Pow(g, Val 2.))
    | Pow (f, Val 2.) -> Mul(Val 2., f)
    | Pow (f, Val k) -> Mul(Val k, Pow(f, Val(k - 1.)))
    | Pow (f, g) -> Mul(Add(Mul(g, diff f x), Mul(Mul(f, Ln f), diff g x)), Pow(f, Sub(g, Val 1.0)))
    | Ln arg -> Div(diff arg x, arg)
    | Sin arg -> Mul(Cos arg, diff arg x)
    | Cos arg -> Mul(Mul(Val -1.0, Sin arg), diff arg x)
    | Exp arg -> Mul(Exp arg, diff arg x)


let rec evalf expr varName value =
    match expr with
    | Val number -> number
    | Var variable when variable = varName -> value
    | Var var -> failwith $"Переменная '%s{var}' не разрешена."
    | Add (a, b) -> (evalf a varName value) + (evalf b varName value)
    | Sub (a, b) -> (evalf a varName value) - (evalf b varName value)
    | Mul (a, b) -> (evalf a varName value) * (evalf b varName value)
    | Div (a, b) -> (evalf a varName value) / (evalf b varName value)
    | Pow (a, b) -> Math.Pow(evalf a varName value, evalf b varName value)
    | Ln arg -> log (evalf arg varName value)
    | Sin arg -> sin (evalf arg varName value)
    | Cos arg -> cos (evalf arg varName value)
    | Exp arg -> exp (evalf arg varName value)


let rec show expr =
    match expr with
    | Val number -> string number
    | Var var -> var
    | Add (a, b) -> $"{show a}+{show b}"
    | Sub (a, b) -> $"{show a}-{show b}"
    | Mul (a, b) -> $"{show a}*{show b}"
    | Div (a, b) -> $"{show a}/{show b}"
    | Pow (a, b) -> $"{show a}**({show b})"
    | Ln arg -> $"Ln({show arg})"
    | Sin arg -> $"Sin({show arg})"
    | Cos arg -> $"Cos({show arg})"
    | Exp arg -> $"Exp({show arg})"
