interface ErrorBarProps {
    message: string;
}

function ErrorBar({message}: ErrorBarProps){
    return <nav className="error-bar">
        <span>{message}</span>
    </nav>
}

export default ErrorBar;