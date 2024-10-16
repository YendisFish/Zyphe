﻿namespace Zyphe.Parser;

public partial class Parser
{
    public void ConsumeFunctionSignature(VariableIdentifier identifier, bool isStructFunc)
    {
        //making sure we are in the root scope
        //figuring out if we are parsing a let, ref, or private func
        switch (identifier)
        {
            case VariableIdentifier.LET:
            {
                FunctionSignature signature = new FunctionSignature(
                    new Tuple<VariableIdentifier, TypeInfo>(VariableIdentifier.LET, this.ConsumeType()),
                    (string)tokens[index].value,
                    readingPrivateScope,
                    isExtern: readingExtern
                );

                if (tokens[index + 1].type == Token.TokenType.LALLIGATOR)
                {
                    index = index + 2;
                    signature = signature with { generics = this.ConsumeGenerics() };
                }
                
                signature = signature with { arguments = this.ParseArgs() };

                Declaration.FunctionDeclaration declaration = new Declaration.FunctionDeclaration(signature);
                declaration.Scope.parent = currentNode.Scope;
                declaration.isStructFunc = isStructFunc;
                declaration.parent = currentNode;

                currentNode.children.Add(declaration);

                index = index + 2;

                if (signature.isExtern)
                {
                    break;
                }
                
                currentNode = declaration;
                state = ParserState.FUNCTION;

                break;
            }

            case VariableIdentifier.REF:
            {
                index = index + 1;
                FunctionSignature signature = new FunctionSignature(
                    new Tuple<VariableIdentifier, TypeInfo>(VariableIdentifier.REF, this.ConsumeType()),
                    (string)tokens[index].value,
                    readingPrivateScope,
                    this.ParseArgs(),
                    this.ConsumeGenerics(),
                    readingExtern
                );

                Declaration.FunctionDeclaration declaration = new Declaration.FunctionDeclaration(signature);
                declaration.Scope.parent = currentNode.Scope;
                declaration.isStructFunc = isStructFunc;
                declaration.parent = currentNode;
                
                currentNode.children.Add(declaration);
                
                index = index + 2;

                if (signature.isExtern)
                {
                    break;
                }

                currentNode = declaration;
                state = ParserState.FUNCTION;

                break;
            }
        }

        if (readingPrivateScope)
        {
            readingPrivateScope = false;
        }

        if (readingExtern)
        {
            readingExtern = false;
        }
    }

    public TypeInfo ConsumeType()
    {
        string typeName = (string)tokens[index].value;
        List<GenericUsage>? tGen = null;

        index = index + 1;

        if (tokens[index].type == Token.TokenType.LALLIGATOR)
        {
            tGen = this.ConsumeGenericUsages();
        }

        //this.ReadToToken(Token.TokenType.WORD);

        return new TypeInfo(typeName);
    }
}
