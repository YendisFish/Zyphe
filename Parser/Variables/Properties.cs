﻿namespace Zyphe.Parser;

public partial class Parser
{
    public void ConsumeProp(VariableIdentifier identifier)
    {
        state = ParserState.PROP;
        
        //consume variable name
        string name = (string)tokens[index].value ?? throw new NullReferenceException();

        index = index + 2;
        
        //consume var type
        TypeInfo tInf = this.ConsumeVarType(); //idk if the pipe works here?
        
        //construct variable signature
        VariableInfo vInf = new(identifier, name, tInf, true, readingPrivateScope);
        Declaration.VariableDeclaration declaration = new(vInf, null); // this is a prop so it cannot have an initializer outside of the constructor
        
        declaration.Scope = currentNode.Scope;
        declaration.Scope.parent = currentNode.Scope;
        declaration.Scope.returnNode = currentNode;
        declaration.Scope.returnState = state;
        declaration.parent = currentNode;
        
        declared.Props.Add(declaration);
        currentNode.children.Add(declaration);
        currentNode = declaration;
    }

    public void ConsumeGetter()
    {
        Declaration.VariableDeclaration decl = (Declaration.VariableDeclaration)currentNode;
        
        switch(tokens[index].type)
        {
            case Token.TokenType.SEMICOLON:
            {
                decl.left.getter = new Declaration.FunctionDeclaration(FunctionSignature.GetterDefault(decl));
                
                //explicitly hand control back to the variable
                state = ParserState.PROP;
                
                break;
            }
                
            case Token.TokenType.LBRACE:
            {
                FunctionSignature signature = new FunctionSignature(new(decl.left.vType, decl.left.type),
                    decl.left.name + "_getter", decl.left.isPrivate, new());
                Declaration.FunctionDeclaration func = new Declaration.FunctionDeclaration(signature);
                
                decl.left.getter = func;

                func.Scope.returnState = ParserState.PROP;
                func.Scope.returnNode = decl;
                currentNode = func;
                
                break;
            }
        }
        
        this.Next();
        
        if (readingPrivateScope)
        {
            readingPrivateScope = false;
        }
        
        state = ParserState.GETTER;
    }
    
    public void ConsumeSetter()
    {
        Declaration.VariableDeclaration decl = (Declaration.VariableDeclaration)currentNode;
        
        switch(tokens[index].type)
        {
            case Token.TokenType.SEMICOLON:
            {
                decl.left.setter = new Declaration.FunctionDeclaration(FunctionSignature.SetterDefault(decl));
                    
                //explicitly hand control back to the variable
                state = ParserState.PROP;
                
                break;
            }
                
            case Token.TokenType.LBRACE:
            {
                FunctionSignature signature = new FunctionSignature(new(decl.left.vType, decl.left.type),
                    decl.left.name + "_setter", decl.left.isPrivate, new()); //todo : Implement function arguments to cover setter inputs!
                Declaration.FunctionDeclaration func = new Declaration.FunctionDeclaration(signature);
                
                decl.left.setter = func;

                func.Scope.returnState = ParserState.PROP;
                func.Scope.returnNode = decl;
                currentNode = func;
                
                break;
            }
        }
        
        this.Next();
        
        state = ParserState.SETTER;
    }
}