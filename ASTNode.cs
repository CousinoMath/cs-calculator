using System;
using System.Collections.Generic;

abstract class ASTNode {
    abstract public double evaluate(Dictionary<string, double> memory);
}

class ASTPlus : ASTNode {
    private List<ASTNode> arguments;

    public ASTPlus(List<ASTNode> args) {
        this.arguments = args;
    }

    override public double evaluate(Dictionary<string, double> memory) {
        double accum = 0.0;
        foreach (ASTNode argument in arguments) {
            accum += argument.evaluate(memory);
        }
        return accum;
    }
}

class ASTTimes : ASTNode {
    private List<ASTNode> arguments;

    public ASTTimes(List<ASTNode> args) {
        this.arguments = args;
    }

    override public double evaluate(Dictionary<string, double> memory) {
        double accum = 1.0;
        foreach (ASTNode argument in arguments) {
            accum *= argument.evaluate(memory);
        }
        return accum;
    }
}

class ASTPower : ASTNode {
    private ASTNode baseNode;
    private ASTNode exponent;

    public ASTPower(ASTNode baseNode, ASTNode exponent) {
        this.baseNode = baseNode;
        this.exponent = exponent;
    }

    override public double evaluate(Dictionary<string, double> memory) {
        double baseValue = this.baseNode.evaluate(memory);
        double expValue = this.exponent.evaluate(memory);
        return Math.Pow(baseValue, expValue);
    }
}

class ASTNumber : ASTNode {
    private double value;

    public ASTNumber(double value) {
        this.value = value;
    }

    override public double evaluate(Dictionary<string, double> memory) {
        return this.value;
    }
}

class ASTVariable : ASTNode {
    private string name;

    public ASTVariable(string name) {
        this.name = name;
    }

    override public double evaluate(Dictionary<string, double> memory) {
        if (memory.TryGetValue(this.name, out double value)) {
            return value;
        } else {
            return Double.NaN;
        }
    }
}

class ASTAssign : ASTNode {
    private string name;
    private ASTNode expression;

    public ASTAssign(string name, ASTNode expression) {
        this.name = name;
        this.expression = expression;
    }

    override public double evaluate(Dictionary<string, double> memory) {
        double value = this.expression.evaluate(memory);
        if (memory.ContainsKey(name)) {
            memory.Remove(name);
        }
        memory.Add(name, value);
        return value;
    }
}

class ASTFunction : ASTNode {
    private string name;
    private ASTNode argument;

    public ASTFunction(string name, ASTNode argument) {
        this.name = name;
        this.argument = argument;
    }

    override public double evaluate(Dictionary<string, double> memory) {
        double value = this.argument.evaluate(memory);
        switch (this.name) {
            case "acos": return Math.Acos(value);
            case "asin": return Math.Asin(value);
            case "atan": return Math.Atan(value);
            case "cos": return Math.Cos(value);
            case "exp": return Math.Exp(value);
            case "log": return Math.Log(value);
            case "sin": return Math.Sin(value);
            case "tan": return Math.Tan(value);
            default: return Double.NaN;
        }
    }
}

class ASTConstant : ASTNode {
    private string name;

    public ASTConstant(string name) {
        this.name = name;
    }

    override public double evaluate(Dictionary<string, double> memory) {
        switch (this.name) {
            case "pi": return Math.PI;
            case "e": return Math.E;
            default: return Double.NaN;
        }
    }
}
