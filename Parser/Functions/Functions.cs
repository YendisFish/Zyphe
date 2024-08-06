namespace Zyphe.Parser;

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
                    readingPrivateScope
                );

                Declaration.FunctionDeclaration declaration = new Declaration.FunctionDeclaration(signature);
                declaration.Scope.parent = currentNode.Scope;
                declaration.isStructFunc = isStructFunc;
                declaration.parent = currentNode;

                currentNode.children.Add(declaration);
                this.ReadToToken(Token.TokenType.LBRACE); // CHANGE TO LBRACE | EQUALS
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
                    readingPrivateScope
                );

                Declaration.FunctionDeclaration declaration = new Declaration.FunctionDeclaration(signature);
                declaration.Scope.parent = currentNode.Scope;
                declaration.isStructFunc = isStructFunc;
                declaration.parent = currentNode;

                currentNode.children.Add(declaration);
                this.ReadToToken(Token.TokenType.LBRACE); // CHANGE TO LBRACE | EQUALS
                currentNode = declaration;
                state = ParserState.FUNCTION;

                break;
            }
        }


        if (readingPrivateScope)
        {
            readingPrivateScope = false;
        }
    }

    public TypeInfo ConsumeType()
    {
        string typeName = (string)tokens[index].value;

        if (tokens[index].type == Token.TokenType.LALLIGATOR)
        {
            // todo: implement generic parsing
            index = index + 1; //take this out and have the generics leave you on the word token
        }

        this.ReadToToken(Token.TokenType.WORD);

        return new TypeInfo(typeName);
    }
}