import {Button, Modal, FormGroup, FormControl, FormLabel, Row, Col, Card, Dropdown} from "react-bootstrap";
import { useEffect, useState } from "react";
import type {ExpenseFilters} from "./client";
import * as client from "./client";

export default function MyExpenses({myExpenses, setMyExpenses} : {
    myExpenses: any[], 
    setMyExpenses: React.Dispatch<React.SetStateAction<any[]>>}) {
    
    // Modal Control
    const [showForm, setShowForm] = useState(false);

    // User entered filters
    const [amount, setAmount] = useState<number | null>(null);
    const [type, setType] = useState<string | null>(null);
    const [date, setDate] = useState<string | null>(null);

    // Reset and Error handling
    const [reset, setReset] = useState(false);

    // Filter dropdowns
    const [types, setTypes] = useState<any>([]);

    const [expenseToDelete, setExpenseToDelete] = useState(-1);
    const [expenseToUpdate, setExpenseToUpdate] = useState<any | null>(null);

    // Fetch filtered or all expenses from db
    const fetchExpenses = async () => {
        // If any filter is applied, call API endpoint for filtered expenses
        if (amount || type || date) {
            const filters: ExpenseFilters = {
                amount: amount,
                type: type,
                date: date
            }

            const expenses = await client.findFilteredExpenses(filters);

            // Newest expenses first
            expenses.sort((a: any, b: any) => new Date(b.date).getDate() - new Date(a.date).getDate());

            setMyExpenses(expenses);
            setShowForm(false);
            return;
        }

        const expenses = await client.findMyExpenses();
        expenses.sort((a: any, b: any) => new Date(b.date).getDate() - new Date(a.date).getDate());
        setMyExpenses(expenses);
    }

    // Fetch all user expense types
    const fetchTypes = async () => {
        const expenseTypes = await client.findExpenseTypes();
        setTypes(expenseTypes);
    }


    // Delete expense from db
    const deleteExpense = async (expenseId: number) => {
        await client.deleteExpense(expenseId);
        fetchExpenses();
        setExpenseToDelete(-1);
    }

    const updateExpense = async () => {
        await client.UpdateUserExpense(expenseToUpdate);
        fetchExpenses();
        setExpenseToUpdate(null);
    }

    // Show DateTime as date
    const formatTime = (date: string) => {
        const newDate = new Date(date);
        const readable = newDate.toLocaleDateString();
        return readable;
    }

    // Clear filters
    const resetFilters = () => {
        setAmount(null);
        setType(null);
        setDate(null);
        setReset(true);
    }

    useEffect(() => {
        fetchExpenses();
        fetchTypes();
    }, [])

    // UseEffect for resetting filters
    useEffect(() => {
        const allCleared =
            amount === null &&
            type === null &&
            date === null;

        if (reset && allCleared) {
            fetchExpenses();
            setReset(false);
        }
    }, [amount, type, date, reset])

    return (
        <div id="da-my-expenses" className="mt-3 col-sm-8">
            <Button onClick={() => setShowForm(true)} className="btn btn-warning mb-3 me-2">
                Filter Expenses
            </Button>
            <Button onClick={resetFilters} className="btn btn-danger mb-3">
                Reset Filters
            </Button>

            {/*Modal for filters*/}
            <Modal show={showForm} onHide={() => setShowForm(false)} centered size="lg">
                <Modal.Header closeButton>
                    <Modal.Title>Filter Expenses</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <div className="filter-expenses">
                        <FormGroup as={Row} className="mb-3">
                            <FormLabel column sm={4}>Minimum Expense Amount</FormLabel>
                            <Col sm={7}>
                                <FormControl
                                    type="number"
                                    value={amount === null ? "" : amount}
                                    step="0.01"
                                    min="0.01"
                                    placeholder="Minimum Expense Amount"
                                    onChange={(e) => setAmount(e.target.value === "" ? null : Number(e.target.value))}
                                />
                            </Col>
                        </FormGroup>
                        <FormGroup as={Row} className="mb-3">
                            <FormLabel column sm={4}>Date</FormLabel>
                            <Col sm={7}>
                                <FormControl
                                    type="date"
                                    value={date === null ? "" : date}
                                    onChange={(e) => setDate(e.target.value === "" ? null : e.target.value)}
                                />
                            </Col>
                        </FormGroup>
                        <FormGroup as={Row} className="d-flex align-items-center mb-2">
                            <FormLabel column sm={4}>Expense Type</FormLabel>
                            <Col sm={7}>
                                <select onChange={(e) => setType(e.target.value)}
                                    className="form-control mb-2" id="da-app">
                                    <option value=""></option>
                                    {types.map((type: any) =>
                                        <option value={type}>{type}</option>
                                    )}
                                </select>
                            </Col>
                        </FormGroup>
                        <Button onClick={fetchExpenses} className="btn btn-primary">
                            Filter Expenses
                        </Button>
                    </div>
                </Modal.Body>
            </Modal>

            {/*Render individual delivery details on cards*/}
            <Row>
                {myExpenses.map((expense: any) => 
                    <Col sm={6}>
                        <Card className="mb-3 text-start user-expense-card">
                            <Card.Body style={{ padding: '0.5rem' }}>
                                {/*Fix dropdown menu to top right corner of card*/}
                                <div style={{ position: 'absolute', top: '0.5rem', right: '0.5rem' }}>
                                    {/*Delete expense dropdown toggle*/}
                                    <Dropdown>
                                        <Dropdown.Toggle variant="secondary" size="sm">
                                            &#x22EE;
                                        </Dropdown.Toggle>

                                        <Dropdown.Menu>
                                            <Dropdown.Item onClick={() => setExpenseToDelete(expense.id)} 
                                                className="text-danger">
                                                Delete Expense
                                            </Dropdown.Item>
                                            <Dropdown.Item onClick={() => setExpenseToUpdate(expense)}
                                                className="text-warning">
                                                Update Expense
                                            </Dropdown.Item>
                                        </Dropdown.Menu>
                                    </Dropdown>
                                </div>

                                <Card.Title className="fw-bold">Amount: ${expense.amount.toFixed(2)}</Card.Title>
                                <Card.Text>
                                    <strong>Date:</strong> {formatTime(expense.date)} {" "} <br />
                                    <strong>Type:</strong> {expense.type} {" "} <br />
                                    <strong>Notes:</strong> {expense.notes} {" "} <br />
                                </Card.Text>

                                {/*Modal to confirm expense deletion*/}
                                <>
                                    <Modal show={expenseToDelete !== -1} 
                                        onHide={() => setExpenseToDelete(-1)} centered size="lg">
                                        <Modal.Header closeButton>
                                            <Modal.Title>Confirm Deletion</Modal.Title>
                                        </Modal.Header>
                                        <Modal.Body>Are you sure you want to delete this expense?</Modal.Body>
                                        <Modal.Footer>
                                            <Button variant="secondary" onClick={() => setExpenseToDelete(-1)}>
                                                Cancel
                                            </Button>
                                            <Button
                                                variant="danger"
                                                onClick={() => {
                                                    deleteExpense(expenseToDelete);
                                                }}
                                            >
                                                Delete
                                            </Button>
                                        </Modal.Footer>
                                    </Modal>

                                    <Modal show={expenseToUpdate !== null} onHide={() => setExpenseToUpdate(null)} centered size="lg">
                                        <Modal.Header closeButton>
                                            <Modal.Title>Update Expense</Modal.Title>
                                        </Modal.Header>
                                        <Modal.Body>
                                            {expenseToUpdate &&
                                            <div id="add-expense-details">
                                                <FormGroup as={Row} className="d-flex align-items-center mb-2">
                                                    <FormLabel column sm={4} className="me-3">Expense Amount</FormLabel>
                                                    <Col sm={7}>
                                                        <FormControl 
                                                            type="number"
                                                            min="0.01"
                                                            step="0.01"
                                                            placeholder="Expense Amount"
                                                            defaultValue={expenseToUpdate.amount}
                                                            onChange={(e) => setExpenseToUpdate({...expenseToUpdate, amount: e.target.value})}
                                                        />
                                                    </Col>
                                                </FormGroup>
                                                <FormGroup as={Row} className="d-flex align-items-center mb-2">
                                                    <FormLabel column sm={4} className="me-3">Date</FormLabel>
                                                    <Col sm={7}>
                                                        <FormControl
                                                            type="date"
                                                            defaultValue={expenseToUpdate.date ? new Date(expenseToUpdate.date).toISOString().split('T')[0] : ''}
                                                            onChange={(e) => setExpenseToUpdate({ ...expenseToUpdate, date: e.target.value })}
                                                        />
                                                    </Col>
                                                </FormGroup>
                                                <FormGroup as={Row} className="d-flex align-items-center mb-2">
                                                    <FormLabel column sm={4} className="me-3">Expense Type</FormLabel>
                                                    <Col sm={7}>
                                                        <FormControl 
                                                            type="text"
                                                            placeholder="Expense Type"
                                                            defaultValue={expenseToUpdate.type}
                                                            onChange={(e) => setExpenseToUpdate({...expenseToUpdate, type: e.target.value})}
                                                        />
                                                    </Col>
                                                </FormGroup>
                                                <FormGroup as={Row} className="d-flex align-items-center mb-2">
                                                    <FormLabel column sm={4} className="me-3">Expense Notes</FormLabel>
                                                    <Col sm={7}>
                                                        <FormControl 
                                                            type="text"
                                                            placeholder="Expense Notes"
                                                            defaultValue={expenseToUpdate.notes}
                                                            onChange={(e) => setExpenseToUpdate({...expenseToUpdate, notes: e.target.value})}
                                                        />
                                                    </Col>
                                                </FormGroup>
                                                <Button onClick={updateExpense} className="btn btn-primary">
                                                    Update Expense
                                                </Button>
                                            </div>
                                            }
                                        </Modal.Body>
                                    </Modal>
                                </>
                            </Card.Body>
                        </Card>
                    </Col>
                )}
            </Row>
        </div>
    );
}