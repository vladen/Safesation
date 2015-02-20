using System;

namespace Safesation
{
    public class Branch<T>
    {
        #region METHODS

        public Branch<T, TResult> Case<TResult>(T value, TResult result)
        {
            return new Branch<T, TResult>(
                (option, otherwise) =>
                    option.Equals(value)
                        ? new Option<TResult>(result)
                        : otherwise(option));
        }

        public Branch<T, TResult> Case<TResult>(Func<T, bool> predicate, TResult result)
        {
            return new Branch<T, TResult>(
                (option, otherwise) =>
                    predicate != null && predicate(option)
                        ? new Option<TResult>(result)
                        : otherwise(option));
        }

        public Branch<T, TResult> Fail<TResult>(TResult result)
        {
            return new Branch<T, TResult>(
                (option, otherwise) =>
                    option.IsFail
                        ? new Option<TResult>(result)
                        : otherwise(option));
        }

        public Branch<T, TResult> None<TResult>(TResult result)
        {
            return new Branch<T, TResult>(
                (option, otherwise) =>
                    option.IsNone
                        ? new Option<TResult>(result)
                        : otherwise(option));
        }

        public Branch<T, TResult> Some<TResult>(TResult result)
        {
            return new Branch<T, TResult>(
                (option, otherwise) =>
                    option.IsSome
                        ? new Option<TResult>(result)
                        : otherwise(option));
        } 

        #endregion
    }

    public class Branch<TValue, TResult>
    {
        #region CONSTRUCTORS

        public Branch(Func<Option<TValue>, Func<Option<TValue>, Option<TResult>>, Option<TResult>> @case)
        {
            _case = @case ?? None;
        } 

        #endregion

        #region FIELDS

        private readonly Func<Option<TValue>, Func<Option<TValue>, Option<TResult>>, Option<TResult>> _case; 

        #endregion

        #region METHODS

        public Branch<TValue, TResult> Case(TValue value, TResult result)
        {
            return new Branch<TValue, TResult>(
                (option, otherwise) =>
                    option.Equals(value)
                        ? new Option<TResult>(result)
                        : otherwise(option));
        }

        public Branch<TValue, TResult> Case(Func<TValue, bool> predicate, TResult result)
        {
            return new Branch<TValue, TResult>(
                (option, otherwise) =>
                    predicate != null && predicate(option)
                        ? new Option<TResult>(result)
                        : otherwise(option));
        }

        public Branch<TValue, TResult> Fail(TResult result)
        {
            return new Branch<TValue, TResult>(
                (option, otherwise) =>
                    option.IsFail
                        ? new Option<TResult>(result)
                        : otherwise(option));
        }

        public Option<TResult> Invoke(Option<TValue> option)
        {
            return _case(option, None);
        }

        private static Option<TResult> None(Option<TValue> option)
        {
            return new Option<TResult>();
        }

        private static Option<TResult> None(Option<TValue> option, Func<Option<TValue>, Option<TResult>> otherwise)
        {
            return otherwise == null ? new Option<TResult>() : otherwise(option);
        }

        public Branch<TValue, TResult> None(TResult result)
        {
            return new Branch<TValue, TResult>(
                (option, otherwise) =>
                    option.IsNone
                        ? new Option<TResult>(result)
                        : otherwise(option));
        }

        public Branch<TValue, TResult> Some(TResult result)
        {
            return new Branch<TValue, TResult>(
                (option, otherwise) =>
                    option.IsSome
                        ? new Option<TResult>(result)
                        : otherwise(option));
        } 

        #endregion
    }
}