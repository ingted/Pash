﻿// Copyright (C) Pash Contributors. License: GPL/BSD. See https://github.com/Pash-Project/Pash/
using System;
using System.Linq;
using System.Collections.Generic;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Extensions.Queue;
using Extensions.Reflection;
using System.Reflection;

namespace System.Management.Automation.Language
{
    static class _
    {
        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> source)
        {
            return new ReadOnlyCollection<T>(new List<T>(source ?? new T[] { }));
        }

        [DebuggerStepThrough]
        public static AstVisitAction Visit(this AstVisitor astVisitor, Ast ast)
        {
            var dispatchMethodInfos = from dmi in astVisitor.GetType().GetMethods()
                                      where dmi.Name.StartsWith("Visit")
                                      let parameterType = dmi.GetParameters().Single().ParameterType
                                      where parameterType.IsAssignableFrom(ast)
                                      orderby parameterType.GetDerivationRank(ast)
                                      select dmi;

            if (dispatchMethodInfos.Any())
            {
                try
                {
                    return (AstVisitAction)dispatchMethodInfos.First().Invoke(astVisitor, new[] { ast });
                }
                catch (TargetInvocationException targetInvocationException)
                {
                    throw targetInvocationException.InnerException;
                }
            }

            else throw new InvalidOperationException(ast.ToString());
        }
    }

    public abstract class Ast
    {
        protected Ast(IScriptExtent extent) { this.Extent = extent; }

        public IScriptExtent Extent { get; private set; }
        public Ast Parent { get; private set; }

        public Ast Find(Func<Ast, bool> predicate, bool searchNestedScriptBlocks) { throw new NotImplementedException(this.ToString()); }

        public IEnumerable<Ast> FindAll(Func<Ast, bool> predicate, bool searchNestedScriptBlocks) { throw new NotImplementedException(this.ToString()); }

        public override string ToString() { throw new NotImplementedException(); }

        internal virtual IEnumerable<Ast> Children { get { yield break; } }

        [DebuggerStepThrough]
        public void Visit(AstVisitor astVisitor)
        {
            Queue<Ast> queue = new Queue<Ast>();

            queue.Enqueue(this);

            while (queue.Any())
            {
                var nextAst = queue.Dequeue();
                AstVisitAction astVisitAction = astVisitor.Visit(nextAst);

                switch (astVisitAction)
                {
                    case AstVisitAction.Continue:
                        queue.EnqueueAll(nextAst.Children);
                        break;

                    case AstVisitAction.SkipChildren:
                        break;

                    case AstVisitAction.StopVisit:
                        return;

                    default:
                        throw new InvalidOperationException(astVisitAction.ToString());
                }
            }
        }

        public object Visit(ICustomAstVisitor astVisitor)
        {
            throw new NotImplementedException(this.ToString());
        }
    }
}
